using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TnyFramework.Codec;
using TnyFramework.Codec.Newtonsoft.Json;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Namespace.Algorithm.XXHash3;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd.Test
{

    public class EtcdNamespaceExplorerTest
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<EtcdNamespaceExplorerTest>();

        private static INamespaceExplorerFactory _NAMESPACE_EXPLORER_FACTORY;

        private static readonly ObjectCodecFactory OBJECT_CODEC_FACTORY = new JsonObjectCodecFactory();

        private static readonly ObjectCodecAdapter OBJECT_CODEC_ADAPTER = new(new List<ObjectCodecFactory> {OBJECT_CODEC_FACTORY});

        private static INamespaceExplorer explorer;

        private const string HEAD = "/ON_Test/";

        private const string HEAD_OTHER = "/ON_Test/ON_Test_OTHER/";

        private const string HASHING_PATH = "/ON_Test/ON_Hashing/";

        private const string PLAYER_NODE_1_KEY = "/ON_Test/namespace/player/node1";

        private const string PLAYER_NODE = "/ON_Test/namespace/player/";

        private const string OTHER_PLAYER_NODE = "/ON_Test_OTHER/namespace/player/";

        [OneTimeSetUp]
        public void Init()
        {
            _NAMESPACE_EXPLORER_FACTORY = new EtcdNamespaceExplorerFactory(new EtcdConfig {
                Endpoints = "http://127.0.0.1:2379"
            }, OBJECT_CODEC_ADAPTER);
            explorer = _NAMESPACE_EXPLORER_FACTORY.Create();
        }

        [SetUp]
        public async Task Setup()
        {
            await explorer.RemoveAll(HEAD);
            await explorer.RemoveAll(HASHING_PATH);
            await explorer.RemoveAll(HEAD_OTHER);
        }

        private static readonly ObjectMimeType<Player> MINE_TYPE = ObjectMimeType.Of<Player>(MimeTypes.JSON);

        [Test]
        public async Task Get()
        {
            var playerNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(playerNode);
            var player = new Player("Lucy", 100);
            var savePlayerNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player);
            var savePlayer = savePlayerNode.Value;
            Assert.AreEqual(player, savePlayer);

            playerNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            var getPlayer = playerNode.Value;
            Assert.AreEqual(player, getPlayer);
        }

        [Test]
        public async Task FindAll()
        {
            var players = new List<Player>();
            for (var i = 0; i < 10; i++)
            {
                var player = new Player(PLAYER_NODE + "PLA_" + i, 10 + i);
                await explorer.Save(player.Name, MINE_TYPE, player);
                players.Add(player);
            }
            var findList = await explorer.FindAll(PLAYER_NODE, MINE_TYPE);
            Assert.AreEqual(players.Count, findList.Count);
            foreach (var node in findList)
            {
                Assert.IsTrue(players.Contains(node.Value));
            }
        }

        [Test]
        public async Task LesseeTest()
        {
            var leased = false;
            var closed = false;
            var renew = false;

            var lessee = await explorer.Lease("Test1", 3);
            lessee.LeaseEvent.Add(_ => leased = true);
            lessee.CompletedEvent.Add(_ => closed = true);
            lessee.RenewEvent.Add(_ => renew = true);

            await lessee.Lease();
            await Task.Delay(1000);
            Assert.IsTrue(leased);
            Assert.IsTrue(renew);

            var players = new List<Player>();
            for (var i = 0; i < 10; i++)
            {
                var player = new Player(PLAYER_NODE + "PLA_" + i, 10 + i);
                players.Add(player);
                await explorer.Save(player.Name, MINE_TYPE, player, lessee);
            }

            await lessee.Revoke();
            await Task.Delay(100);
            Assert.IsTrue(closed);
            foreach (var player in players)
            {
                Assert.IsNull(await explorer.Get(player.Name, MINE_TYPE));
            }
        }

        [Test]
        public async Task AllNodeWatcherTest()
        {
            var players = new List<Player>();
            var loadSet = new HashSet<Player>();
            var createSet = new HashSet<Player>();
            var updateSet = new HashSet<Player>();
            var deleteSet = new HashSet<Player>();

            var watched = false;
            var closed = false;
            const int loadSize = 5;
            for (var i = 0; i < 10; i++)
            {
                var player = new Player(PLAYER_NODE + "PLA_" + i, 10 + i);
                players.Add(player);
                if (i < loadSize)
                {
                    await explorer.Save(player.Name, MINE_TYPE, player);
                }
            }

            var watcher = explorer.AllNodeWatcher(PLAYER_NODE, MINE_TYPE);
            watcher.LoadEvent.Add((_, nodes) => {
                Assert.AreEqual(nodes.Count, loadSize);
                foreach (var node in nodes)
                {
                    Assert.IsTrue(players.Contains(node.Value));
                    Assert.IsTrue(loadSet.Add(node.Value));
                }
            });
            watcher.CreateEvent.Add((_, node) => {
                Assert.IsTrue(createSet.Add(node.Value));
                Assert.IsTrue(players.Contains(node.Value));
            });
            watcher.UpdateEvent.Add((_, node) => {
                Assert.IsTrue(updateSet.Add(node.Value));
                Assert.IsTrue(loadSet.Contains(node.Value));
                Assert.IsTrue(players.Contains(node.Value));
            });
            watcher.DeleteEvent.Add((_, node) => {
                Assert.IsTrue(players.Contains(node.Value));
                Assert.IsTrue(deleteSet.Add(node.Value));
            });
            watcher.WatchEvent.Add(_ => { watched = true; });
            watcher.CompleteEvent.Add(_ => { closed = true; });
            await watcher.Watch();
            await Task.Delay(100);

            Assert.IsTrue(watched);

            foreach (var player in players)
            {
                await explorer.Save(player.Name, MINE_TYPE, player);
            }
            await Task.Delay(100);

            foreach (var player in players)
            {
                await explorer.Remove(player.Name);
            }

            await Task.Delay(100);

            Assert.AreEqual(loadSize, loadSet.Count);
            Assert.AreEqual(players.Count - loadSize, createSet.Count);
            Assert.AreEqual(loadSize, updateSet.Count);
            Assert.AreEqual(players.Count, deleteSet.Count);

            await watcher.Unwatch();
            await Task.Delay(100);
            Assert.IsTrue(closed);
        }

        [Test]
        public async Task NodeWatcherTest()
        {
            var loadSize = 0;
            var createSize = 0;
            var updateSize = 0;
            var deleteSize = 0;

            var watchPath = PLAYER_NODE + "PLAYER";
            var playerLoad = new Player(watchPath, 100);
            await explorer.Save(playerLoad.Name, MINE_TYPE, playerLoad);

            var watched = false;
            var closed = false;
            var playerWatched = new Player(watchPath, 200);
            var playerUnwatched = new Player(watchPath + "_1", 300);

            var watcher = explorer.NodeWatcher(watchPath, MINE_TYPE);
            watcher.LoadEvent.Add((_, nodes) => {
                Assert.AreEqual(nodes.Count, 1);
                foreach (var node in nodes)
                {
                    Assert.AreEqual(playerLoad, node.Value);
                }
                loadSize++;
            });
            watcher.CreateEvent.Add((_, node) => {
                LOGGER.LogInformation($"CreateEvent {node.Value}");
                Assert.AreEqual(node.Value, playerWatched);
                createSize++;
            });
            watcher.UpdateEvent.Add((_, node) => {
                LOGGER.LogInformation($"UpdateEvent {node.Value}");
                Assert.AreEqual(node.Value, playerWatched);
                updateSize++;
            });
            watcher.DeleteEvent.Add((_, node) => {
                deleteSize++;
                Assert.AreEqual(node.Value, deleteSize == 1 ? playerLoad : playerWatched);
            });
            watcher.WatchEvent.Add(_ => { watched = true; });
            watcher.CompleteEvent.Add(_ => { closed = true; });
            await watcher.Watch();
            await Task.Delay(100);

            Assert.IsTrue(watched);

            await explorer.Remove(playerLoad.Name);
            LOGGER.LogInformation("1");
            await explorer.Save(playerWatched.Name, MINE_TYPE, playerWatched);
            LOGGER.LogInformation("2");
            await explorer.Save(playerWatched.Name, MINE_TYPE, playerWatched);

            LOGGER.LogInformation("3");
            await explorer.Save(playerUnwatched.Name, MINE_TYPE, playerUnwatched);
            await explorer.Save(playerUnwatched.Name, MINE_TYPE, playerUnwatched);
            await explorer.Save(playerUnwatched.Name, MINE_TYPE, playerUnwatched);

            await explorer.Remove(playerWatched.Name);

            await Task.Delay(100);
            Assert.AreEqual(1, loadSize);
            Assert.AreEqual(1, createSize);
            Assert.AreEqual(1, updateSize);
            Assert.AreEqual(2, deleteSize);

            await watcher.Unwatch();
            await Task.Delay(100);
            Assert.IsTrue(closed);
        }

        [Test]
        public async Task TestPublishSubscribe()
        {
            var nameHasher = HashAlgorithmHasher.Hasher<Player>(p => p.Name, XxHash3HashAlgorithm.XXH3_HASH_32);
            var subscriber = explorer.HashingSubscriber(HASHING_PATH, nameHasher.Max, MINE_TYPE);
            var publisher = explorer.HashingPublisher<string, Player>(HASHING_PATH, nameHasher.Max, nameHasher, MINE_TYPE);
            var maxSlot = (long) (ulong.MaxValue >> 32);
            var toSlot = maxSlot / 2;
            var playerList = new List<Player>();
            var prePlayerMap = new Dictionary<long, Player>();
            var prePlayerList = new List<Player>();
            var opPlayerList = new List<Player>();

            var watchedList = new List<Player>();

            var loadList = new List<Player>();
            var createList = new List<Player>();
            var updateList = new List<Player>();
            var deleteList = new List<Player>();
            var checkList = new List<List<Player>> {loadList, createList, updateList, deleteList};

            for (var i = 0; i < 100; i++)
            {
                var player = new Player("PLA_" + i, 10 + i);
                var hash = nameHasher.Hash(player, 0, maxSlot);
                if (hash <= toSlot)
                {
                    if (prePlayerMap.TryAdd(hash, player))
                    {
                        prePlayerList.Add(player);
                        await publisher.Publish(player.Name, player);
                    } else
                    {
                        opPlayerList.Add(player);
                    }
                    watchedList.Add(player);
                    LOGGER.LogInformation("watched = " + player.Name + " = " + hash);
                }
                playerList.Add(player);
            }

            subscriber.LoadEvent.Add((_, nodes) => { loadList.AddRange(nodes.Select(n => n.Value)); });
            subscriber.CreateEvent.Add((_, node) => {
                createList.Add(node.Value);
                LOGGER.LogInformation("OnCrate + " + node.Name + " | size = " + createList.Count + " | total size = " + watchedList.Count);
            });
            subscriber.UpdateEvent.Add((_, node) => { updateList.Add(node.Value); });
            subscriber.DeleteEvent.Add((_, node) => { deleteList.Add(node.Value); });

            await subscriber.Subscribe(new List<ShardingRange> {new(0, toSlot, maxSlot)});
            var lessee = await publisher.Lease();
            await Task.Delay(100);

            foreach (var player in playerList)
            {
                await publisher.Publish(player.Name, player);
            }
            await Task.Delay(100);

            Check(checkList, prePlayerList, opPlayerList, prePlayerList, new List<Player>());

            await lessee.Revoke();
            await Task.Delay(100);

            Check(checkList, prePlayerList, opPlayerList, prePlayerList, watchedList);
        }

        private void Check(IReadOnlyList<List<Player>> checkList,
            ICollection<Player> loadList, ICollection<Player> createList, ICollection<Player> updateList, ICollection<Player> deleteList)
        {
            AssertCollection(checkList[0], loadList, "Load");
            AssertCollection(checkList[1], createList, "Create");
            AssertCollection(checkList[2], updateList, "Update");
            AssertCollection(checkList[3], deleteList, "Delete");
        }

        private void AssertCollection(ICollection<Player> expect, ICollection<Player> check, string name)
        {
            Assert.AreEqual(expect.Count, check.Count, name);
            foreach (var player in check)
            {
                Assert.IsTrue(expect.Contains(player), name);
            }
        }

        [Test]
        public async Task TestGetOrAdd()
        {
            var player = new Player(PLAYER_NODE_1_KEY, 100);
            var nameNode = await explorer.GetOrAdd(player.Name, MINE_TYPE, player);
            Assert.AreEqual(player, nameNode.Value);
            var newPlayer = new Player(PLAYER_NODE_1_KEY, 200);
            nameNode = await explorer.GetOrAdd(player.Name, MINE_TYPE, newPlayer);
            Assert.AreEqual(player, nameNode.Value);
        }

        [Test]
        public async Task TestAdd()
        {
            var player = new Player(PLAYER_NODE_1_KEY, 100);
            var nameNode = await explorer.Add(player.Name, MINE_TYPE, player);

            Assert.AreEqual(player, nameNode.Value);
            var newPlayer = new Player(PLAYER_NODE_1_KEY, 200);
            nameNode = await explorer.Add(player.Name, MINE_TYPE, newPlayer);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task TestSave()
        {
            var player = new Player(PLAYER_NODE_1_KEY, 100);
            var nameNode = await explorer.Save(player.Name, MINE_TYPE, player);
            Assert.AreEqual(player, nameNode.Value);
            var newPlayer = new Player(PLAYER_NODE_1_KEY, 200);
            nameNode = await explorer.Save(player.Name, MINE_TYPE, newPlayer);
            Assert.AreEqual(newPlayer, nameNode.Value);
        }

        [Test]
        public async Task TestUpdate()
        {
            var player = new Player(PLAYER_NODE_1_KEY, 100);
            var nameNode = await explorer.Update(player.Name, MINE_TYPE, player);
            Assert.IsNull(nameNode);

            await explorer.Save(player.Name, MINE_TYPE, player);
            var newPlayer = new Player(PLAYER_NODE_1_KEY, 200);
            nameNode = await explorer.Update(newPlayer.Name, MINE_TYPE, newPlayer);
            Assert.AreEqual(newPlayer, nameNode.Value);
        }

        [Test]
        public async Task UpdateIfValue()
        {
            var player1 = new Player(PLAYER_NODE + "PL_2", 100);
            var player2 = new Player(PLAYER_NODE + "PL_1", 102);
            var nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, MINE_TYPE, player1, player2);
            Assert.IsNull(nameNode);

            await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, MINE_TYPE, player2, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, MINE_TYPE, player1, player2);
            Assert.AreEqual(player2, nameNode.Value);
        }

        [Test]
        public async Task UpdateIfValueWithLessee()
        {
            var lessee = await explorer.Lease("updateIfValueWithLessee", 20);
            var player1 = new Player(PLAYER_NODE + "PL_2", 100);
            var player2 = new Player(PLAYER_NODE + "PL_1", 102);

            await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            var nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, MINE_TYPE, player1, player2, lessee);
            Assert.AreEqual(player2, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task TestUpdateIfVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);
            var nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 2, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

        }

        [Test]
        public async Task TestUpdateIfVersionWithLessee()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);

            var lessee = await explorer.Lease("testUpdateIfVersionWithLessee", 20);

            var nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 2, MINE_TYPE, player1, lessee);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, MINE_TYPE, player1, lessee);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, MINE_TYPE, player2, lessee);
            Assert.AreEqual(player2, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task TestUpdateIfMinAndMaxVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);

            // 0 x
            var nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            // 0 -> 1
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            // 1 -> 2
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            // 2 -> 3
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            // 3 x
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);

            // 0 x
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Open, 2, RangeBorder.Open, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            // 0 -> 1
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, -1, RangeBorder.Open, 2, RangeBorder.Open, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            // 1 -> 2
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Open, 2, RangeBorder.Open, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            // 2 x
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Open, 2, RangeBorder.Open, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            // 2 -> 3
            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Open, 3, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 0, RangeBorder.Unlimited, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 0, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 2, RangeBorder.Open, 0, RangeBorder.Unlimited, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Open, 0, RangeBorder.Unlimited, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Unlimited, 0, RangeBorder.Close, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Unlimited, 2, RangeBorder.Close, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 2, RangeBorder.Unlimited, 2, RangeBorder.Open, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Unlimited, 3, RangeBorder.Open, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Unlimited, 3, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);

            nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Unlimited, 3, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

        }

        [Test]
        public async Task TestUpdateIfMinAndMaxVersionWithLessee()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var lessee = await explorer.Lease("testUpdateIfMinAndMaxVersionWithLessee", 3000);

            var nameNode = await explorer.UpdateIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player1, lessee);
            Assert.AreEqual(player1, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task TestUpdateById()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);

            var nameNode = await explorer.UpdateById(PLAYER_NODE_1_KEY, 200, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            nameNode = await explorer.UpdateById(PLAYER_NODE_1_KEY, nameNode.Id, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);
        }

        [Test]
        public async Task TestUpdateByIdWithLessee()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);

            var lessee = await explorer.Lease("testUpdateByIdWithLessee", 3000);

            var nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            nameNode = await explorer.UpdateById(PLAYER_NODE_1_KEY, nameNode.Id, MINE_TYPE, player2, lessee);
            Assert.AreEqual(player2, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);

        }

        [Test]
        public async Task UpdateByIdIfValue()
        {
            var player1 = new Player(PLAYER_NODE + "PL_2", 100);
            var player2 = new Player(PLAYER_NODE + "PL_1", 102);

            var nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, 100, MINE_TYPE, player1, player2);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, MINE_TYPE, player2, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, MINE_TYPE, player1, player2);
            Assert.AreEqual(player2, nameNode.Value);
        }

        [Test]
        public async Task UpdateByIdValueWithLessee()
        {
            var lessee = await explorer.Lease("updateByIdValueWithLessee", 3000);
            var player1 = new Player(PLAYER_NODE + "PL_2", 100);
            var player2 = new Player(PLAYER_NODE + "PL_1", 102);

            var nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, MINE_TYPE, player2, player1, lessee);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, MINE_TYPE, player1, player2, lessee);
            Assert.AreEqual(player2, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task TestUpdateByIdIfVersion()
        {

            var player1 = new Player(PLAYER_NODE + "PL_2", 100);
            var player2 = new Player(PLAYER_NODE + "PL_1", 102);

            var nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, 100, 2, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, 100, 0, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 2, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

        }

        [Test]
        public async Task TestUpdateByIdIfVersionWithLessee()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);

            var lessee = await explorer.Lease("testUpdateByIdIfVersionWithLessee", 3000);

            var nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, 100, 2, MINE_TYPE, player1, lessee);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, 100, 0, MINE_TYPE, player1, lessee);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 2, MINE_TYPE, player2, lessee);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, MINE_TYPE, player2, lessee);
            Assert.AreEqual(player2, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task TestUpdateByIdIfMinAndMaxVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 102);

            // 0 -> 1
            var nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, 100, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            var id = nameNode.Id;

            // 1 -> 2
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 3, RangeBorder.Close, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            // 2 -> 3
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 3, RangeBorder.Close, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            // 3 -> 4
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 3, RangeBorder.Close, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            // 4 x
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 3, RangeBorder.Close, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);
            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            id = nameNode.Id;

            // 1 x
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Open, 3, RangeBorder.Open, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            // 1 -> 2
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Open, 3, RangeBorder.Open, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            // 2 -> 3
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Open, 3, RangeBorder.Open, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            // 3 x
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Open, 3, RangeBorder.Open, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            // 3 -> 4
            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Open, 4, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);
            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 0, RangeBorder.Unlimited, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 0, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 3, RangeBorder.Open, 0, RangeBorder.Unlimited, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 2, RangeBorder.Open, 0, RangeBorder.Unlimited, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);
            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 1, RangeBorder.Close, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 3, RangeBorder.Close, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 3, RangeBorder.Open, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 4, RangeBorder.Open, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 4, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

            await explorer.Remove(PLAYER_NODE_1_KEY);
            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 0, RangeBorder.Unlimited, MINE_TYPE, player2);
            Assert.AreEqual(player2, nameNode.Value);

        }

        [Test]
        public async Task TestUpdateByIdIfMinAndMaxVersionWithLessee()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 200);

            var lessee = await explorer.Lease("testUpdateByIdIfMinAndMaxVersionWithLessee", 3000);

            var nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            var id = nameNode.Id;

            nameNode = await explorer.UpdateByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE, player1, lessee);
            Assert.AreEqual(player1, nameNode.Value);

            await lessee.Shutdown();

            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            Assert.IsNull(nameNode);
        }

        [Test]
        public async Task Remove()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var result = await explorer.Remove(PLAYER_NODE_1_KEY);
            Assert.IsFalse(result);

            await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            result = await explorer.Remove(PLAYER_NODE_1_KEY);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveAll()
        {
            for (var i = 0; i < 5; i++)
            {
                var player = new Player(PLAYER_NODE + "PLA_" + i, 10 + i);
                await explorer.Save(player.Name, MINE_TYPE, player);
            }
            for (var i = 0; i < 5; i++)
            {
                var player = new Player(OTHER_PLAYER_NODE + "PLA_" + i, 10 + i);
                await explorer.Save(player.Name, MINE_TYPE, player);
            }

            await explorer.RemoveAll(PLAYER_NODE);

            var players = await explorer.FindAll(PLAYER_NODE, MINE_TYPE);
            Assert.IsTrue(players.IsEmpty());

            var otherPlayers = await explorer.FindAll(OTHER_PLAYER_NODE, MINE_TYPE);
            Assert.AreEqual(5, otherPlayers.Count);
        }

        [Test]
        public async Task RemoveAllAndGet()
        {
            var players1 = new List<Player>();
            var players2 = new List<Player>();
            for (var i = 0; i < 5; i++)
            {
                var player = new Player(PLAYER_NODE + "PLA_" + i, 10 + i);
                await explorer.Save(player.Name, MINE_TYPE, player);
                players1.Add(player);
            }
            for (var i = 0; i < 5; i++)
            {
                var player = new Player(OTHER_PLAYER_NODE + "PLA_" + i, 10 + i);
                await explorer.Save(player.Name, MINE_TYPE, player);
                players2.Add(player);
            }

            var removes = await explorer.RemoveAllAndGet(PLAYER_NODE, MINE_TYPE);
            Assert.AreEqual(players1.Count, removes.Count);
            foreach (var nameNode in removes)
            {
                Assert.IsTrue(players1.Contains(nameNode.Value));

            }

            var players = await explorer.FindAll(PLAYER_NODE, MINE_TYPE);
            Assert.IsTrue(players.IsEmpty());

            var otherPlayers = await explorer.FindAll(OTHER_PLAYER_NODE, MINE_TYPE);
            Assert.AreEqual(players2.Count, otherPlayers.Count);
            foreach (var nameNode in otherPlayers)
            {
                Assert.IsTrue(players2.Contains(nameNode.Value));
            }

        }

        [Test]
        public async Task TestRemoveIfValue()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 200);

            var nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);
        }

        [Test]
        public async Task TestRemoveIfVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 2, MINE_TYPE);
            Assert.IsNull(nameNode);

            await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 2, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 1, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);
        }

        [Test]
        public async Task TestRemoveIfMinAndMaxVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Close, 1, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Close, 1, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 4, RangeBorder.Close, 5, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 3, RangeBorder.Close, 4, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 1, RangeBorder.Close, 3, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 3, RangeBorder.Open, 5, RangeBorder.Open, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Open, 3, RangeBorder.Open, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 2, RangeBorder.Open, 4, RangeBorder.Open, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 3, RangeBorder.Open, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 2, RangeBorder.Open, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 4, RangeBorder.Close, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 3, RangeBorder.Close, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Unlimited, 3, RangeBorder.Open, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Unlimited, 4, RangeBorder.Open, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Unlimited, 2, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveIf(PLAYER_NODE_1_KEY, 0, RangeBorder.Unlimited, 3, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

        }

        [Test]
        public async Task RemoveById()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var nameNode = await explorer.RemoveById(PLAYER_NODE_1_KEY, 100, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.RemoveById(PLAYER_NODE_1_KEY, 100, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveById(PLAYER_NODE_1_KEY, id, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);
        }

        [Test]
        public async Task TestRemoveByIdAndIfValue()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);
            var player2 = new Player(PLAYER_NODE + "PL_2", 100);

            var nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, 100, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, MINE_TYPE, player2);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, MINE_TYPE, player1);
            Assert.AreEqual(player1, nameNode.Value);
        }

        [Test]
        public async Task TestRemoveByIdAndIfVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, 100, MINE_TYPE, player1);
            Assert.IsNull(nameNode);

            await SaveToVersion(PLAYER_NODE_1_KEY, player1, 2);
            nameNode = await explorer.Get(PLAYER_NODE_1_KEY, MINE_TYPE);
            var id = nameNode.Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 1, MINE_TYPE);
            Assert.IsNull(nameNode);
            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 3, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 2, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);
        }

        [Test]
        public async Task TestRemoveByIdAndIfMinAndMaxVersion()
        {
            var player1 = new Player(PLAYER_NODE + "PL_1", 100);

            var nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, 100, 1, RangeBorder.Close, 1, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.Save(PLAYER_NODE_1_KEY, MINE_TYPE, player1);
            var id = nameNode.Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 1, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            nameNode = await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3);
            id = nameNode.Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 4, RangeBorder.Close, 5, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 2, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 3, RangeBorder.Close, 4, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            id = (await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3)).Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 1, RangeBorder.Close, 3, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            id = (await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3)).Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 3, RangeBorder.Open, 5, RangeBorder.Open, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Open, 3, RangeBorder.Open, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 2, RangeBorder.Open, 4, RangeBorder.Open, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            id = (await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3)).Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 3, RangeBorder.Open, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 2, RangeBorder.Open, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            id = (await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3)).Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 4, RangeBorder.Close, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 3, RangeBorder.Close, 5, RangeBorder.Unlimited, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            id = (await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3)).Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 3, RangeBorder.Open, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 4, RangeBorder.Open, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);

            id = (await SaveToVersion(PLAYER_NODE_1_KEY, player1, 3)).Id;

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 2, RangeBorder.Close, MINE_TYPE);
            Assert.IsNull(nameNode);

            nameNode = await explorer.RemoveByIdAndIf(PLAYER_NODE_1_KEY, id, 0, RangeBorder.Unlimited, 3, RangeBorder.Close, MINE_TYPE);
            Assert.AreEqual(player1, nameNode.Value);
        }

        [Test]
        public void DicTest()
        {
            var dictionary = new Dictionary<string, Player>();
            var p1= new Player("A", 1);
            var p2= new Player("B", 2);
            var p3= new Player("C", 3);
            var p4= new Player("CC", 4);
            dictionary["a"] = p1;
            dictionary["b"] = p2;
            dictionary["c"] = p3;
            Assert.AreEqual(p1, dictionary["a"]);
            Assert.AreEqual(p2, dictionary["b"]);
            Assert.AreEqual(p3, dictionary["c"]);
            dictionary["c"] = p4;
            Assert.AreEqual(p4, dictionary["c"]);
            // Assert.IsNull(dictionary["d"]);
        }

        private static async Task<NameNode<Player>> SaveToVersion(string path, Player player, int version)
        {
            NameNode<Player> node = null;
            for (var i = 0; i < version; i++)
            {
                node = await explorer.Save(path, MINE_TYPE, player);
            }
            return node;
        }
        
        

        // [Test]
        public async Task  HashingTest()
        {
            const int maxSlots = 32;
            const int partitionCount = 6;
            var keyHash = HashAlgorithmHasher.Hasher<string>(maxSlots, XxHash3HashAlgorithm.XXH3_HASH_32);
            var nodeHash = HashAlgorithmHasher.Hasher<PartitionSlot<TestShadingNode>>(p => p.NodeKey, maxSlots, XxHash3HashAlgorithm.XXH3_HASH_32);
            var hashing1 = explorer.NodeHashing("/T2/Nodes", keyHash.Max, keyHash, nodeHash, EtcdNodeHashingMultimapFactory.Default, options => {
                    options.Name = "Harding1";
                    options.PartitionCount = partitionCount;
                });
            var hashing2 = explorer.NodeHashing("/T2/Nodes", keyHash.Max, keyHash, nodeHash, EtcdNodeHashingMultimapFactory.Default, options => {
                options.Name = "Harding2";
                options.PartitionCount = partitionCount;
            });
            
            await hashing1.Start();
            await hashing2.Start();

            await hashing1.Register(new TestShadingNode("Server1"));
            await hashing2.Register(new TestShadingNode("Server2"));
            
        }

    }

    public class Player
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Player()
        {
        }

        public Player(string name, int age)
        {
            Name = name;
            Age = age;
        }

        private bool Equals(Player other)
        {
            return Name == other.Name && Age == other.Age;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Player) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
        }
    }

}
