//
//  文件名称：ItemService.cs
//  简   述：item service
//  创建标识：lrg 2021/9/15
//

#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GF.Game;
using Microsoft.Extensions.Logging;
using Puerts.Api;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Concurrency;

#endregion

namespace puertstest.World.Game
{

    public class ItemService : ItemModelProvider
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<ItemService>();

        private ConcurrentDictionary<int, ItemModelInfo> modelInfoMap;
        private ConcurrentDictionary<ItemType, List<int>> modelInfoListMap;
        private ConcurrentDictionary<int, string> id_alias_map;
        private ConcurrentDictionary<string, int> alias_id_map;
        private ConcurrentDictionary<int, ItemModel> modelMap;

        private IPuertsEnv puertsEnv;

        private static readonly DedicatedThreadPool THREAD_POOL = new DedicatedThreadPool(new DedicatedThreadPoolSettings(1, "ItemServiceThread"));


        private void InitPuerts(IPuertsEnvFactory factory)
        {
            if (puertsEnv != null)
            {
                return;
            }
            lock (this)
            {
                if (puertsEnv != null)
                {
                    return;
                }
                var source = new TaskCompletionSource<bool>();
                THREAD_POOL.QueueUserWorkItem(() => {
                    try
                    {
                        if (puertsEnv != null)
                            return;
                        puertsEnv = factory.CreateEnv();
                        source.SetResult(true);
                    } catch (Exception e)
                    {
                        LOGGER.LogError(e, "InitPuerts exception");
                        source.SetException(e);
                    }
                });
                var _ = source.Task.Result;
            }
        }


        public void Init(IPuertsEnvFactory factory, bool loadAll = false)
        {
            InitPuerts(factory);
            modelInfoMap = new ConcurrentDictionary<int, ItemModelInfo>();
            modelInfoListMap = new ConcurrentDictionary<ItemType, List<int>>();
            id_alias_map = new ConcurrentDictionary<int, string>();
            alias_id_map = new ConcurrentDictionary<string, int>();
            modelMap = new ConcurrentDictionary<int, ItemModel>();
            //获取所有itemModel的概述
            var itemModelInfoList = Eval<List<ItemModelInfo>>(@"
                var ItemModelInfos = require('Config/ItemModelInfos');
                ItemModelInfos.GetItemModelInfos();
            ");
            foreach (var info in itemModelInfoList)
            {
                if (!modelInfoMap.ContainsKey(info.Id))
                {
                    modelInfoMap.TryAdd(info.Id, info);
                    id_alias_map.TryAdd(info.Id, info.Alias);
                    alias_id_map.TryAdd(info.Alias, info.Id);
                } else
                {
                    throw new Exception($"id重复，请检查配置表：{info.Id}");
                }
            }
            if (loadAll)
            {
                foreach (var info in itemModelInfoList)
                {
                    GetModel<ItemModel>(info.Id);
                }
            }

            ////buff model
            //var burnBuffModel = new BuffModel();
            //burnBuffModel.Update(GameItemType.BUFF.Id, BuffType.DOT, 3000, 500);
            //burnBuffModel.AddAbility(GameAbility.Create(GameAbilityType.BUFF_HP_RED, 10));
            //modelMap.Add(burnBuffModel.ItemId, burnBuffModel);

            //var speedBuffModel = new BuffModel();
            //speedBuffModel.Update(GameItemType.BUFF.Id + 1, BuffType.ABILITY_DEC, 3000, 0);
            //speedBuffModel.AddAbility(GameAbility.Create(GameAbilityType.SPEED, -3));
            //modelMap.Add(speedBuffModel.ItemId, speedBuffModel);

            ////Collider model
            //var colliderModel = new GameColliderModel();
            //colliderModel.Update(GameItemType.COLLIDER.Id, GameColliderType.OBB, 1.5f, 0.6f, 0);
            //modelMap.Add(colliderModel.ItemId, colliderModel);

            //colliderModel = new GameColliderModel();
            //colliderModel.Update(GameItemType.COLLIDER.Id + 1, GameColliderType.OBB, 1f, 1f, 0);
            //modelMap.Add(colliderModel.ItemId, colliderModel);

            //colliderModel = new GameColliderModel();
            //colliderModel.Update(GameItemType.COLLIDER.Id + 2, GameColliderType.OBB, 3, 3, 0);
            //modelMap.Add(colliderModel.ItemId, colliderModel);

            //colliderModel = new GameColliderModel();
            //colliderModel.Update(GameItemType.COLLIDER.Id + 3, GameColliderType.OBB, 5, 5, 0);
            //modelMap.Add(colliderModel.ItemId, colliderModel);

            Action action = () => { this.Debug("11"); };

            ////地图 model
            //var worldModel = new WorldMapModel();
            //modelMap.Add(worldModel.ItemId, worldModel);

            ////人物 model
            //var gamerModel = new GamerModel();
            //gamerModel.Update(GameItemType.GAMER.Id, GameItemType.COLLIDER.Id + 1, "demo_role_02");
            //modelMap.Add(gamerModel.ItemId, gamerModel);

            ////怪物 model
            //var monsterModel = new MonsterModel();
            //monsterModel.Update(GameItemType.MONSTER.Id, GameItemType.COLLIDER.Id + 1, "monster", 5);
            //monsterModel.AddSkill(GameItemType.SKILL.Id + 1);
            //modelMap.Add(monsterModel.ItemId, monsterModel);

            //var bossMonsterModel = new MonsterModel();
            //bossMonsterModel.Update(GameItemType.MONSTER.Id + 1, GameItemType.COLLIDER.Id + 1, "boss", 20, true);
            //bossMonsterModel.AddSkill(GameItemType.SKILL.Id);
            //bossMonsterModel.AddSkill(GameItemType.SKILL.Id + 1);
            //bossMonsterModel.AddSkill(GameItemType.SKILL.Id + 2);
            //modelMap.Add(bossMonsterModel.ItemId, bossMonsterModel);

            ////技能 model
            //var fireSkillModel = new SkillModel();
            //fireSkillModel.Update(GameItemType.SKILL.Id, GameSkillType.CAST.Name, 0, 0, 1000 / 15, 0, 0, GameItemType.SKILL_BULLET.Id);
            //modelMap.Add(fireSkillModel.ItemId, fireSkillModel);

            //var whirlWindSkillModel = new SkillModel();
            //whirlWindSkillModel.Update(GameItemType.SKILL.Id + 1, GameSkillType.DEFAULT.Name, 0, 0, 2000, 500, GameItemType.COLLIDER.Id + 2, 0);
            //modelMap.Add(whirlWindSkillModel.ItemId, whirlWindSkillModel);

            //var thunderSkillModel = new SkillModel();
            //thunderSkillModel.Update(GameItemType.SKILL.Id + 2, GameSkillType.CAST.Name, 0, 0, 1000 / 15, 0, 0, GameItemType.SKILL_BULLET.Id + 1);
            //modelMap.Add(thunderSkillModel.ItemId, thunderSkillModel);

            ////技能衍生物 model
            //var fireBulletModel = new SkillBulletModel();
            //fireBulletModel.Update(GameItemType.SKILL_BULLET.Id, GameItemType.COLLIDER.Id, 10, 20, 1);
            //fireBulletModel.AddBuff(GameItemType.BUFF.Id);
            //modelMap.Add(fireBulletModel.ItemId, fireBulletModel);

            //var thunderBulletModel = new SkillBulletModel();
            //thunderBulletModel.Update(GameItemType.SKILL_BULLET.Id + 1, GameItemType.COLLIDER.Id + 3, 20, 0, 0);
            //thunderBulletModel.AddBuff(GameItemType.BUFF.Id + 1);
            //modelMap.Add(thunderBulletModel.ItemId, thunderBulletModel);

            // ItemModels.Ins.Provider = this;
        }


        ItemModel ItemModelProvider.GetItemModel(int id)
        {
            return GetModel<ItemModel>(id);
        }


        T ItemModelProvider.GetItemModel<T>(int id)
        {
            return GetModel<T>(id);
        }


        ItemType ItemModelProvider.GetItemType(int id)
        {
            return GameItemType.ForModelId(id);
        }


        string ItemModelProvider.GetAlias(int id)
        {
            return id_alias_map.Get(id);
        }


        public T GetFunc<T>(string funcStr) where T : Delegate
        {
            return puertsEnv.Delegate<T>(funcStr, "chunk", true);
        }


        private T LoadModel<T>(ItemModelInfo info)
        {
            return Eval<T>($@"
                    var ItemModelInfos = require('Config/ItemModelInfos');
                    ItemModelInfos.GetItemModel('{info.ModelName}', {info.Id});
                    ");
        }


        private T Eval<T>(string script)
        {
            var source = new TaskCompletionSource<T>();
            THREAD_POOL.QueueUserWorkItem(() => {
                try
                {
                    if (puertsEnv == null)
                    {
                        source.SetException(new NullReferenceException("puertsManager is null"));
                        return;
                    }
                    var model = puertsEnv.Eval<T>(script);
                    source.SetResult(model);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "InitPuerts exe {Script} exception", script);
                    source.SetException(e);
                }
            });
            return source.Task.Result;
        }




        private T GetModel<T>(int id) where T : ItemModel
        {
            if (modelMap.TryGetValue(id, out var exist))
                return (T) exist;

            if (modelMap.TryGetValue(id, out exist))
                return (T) exist;
            lock (typeof(T))
            {
                if (modelMap.TryGetValue(id, out exist))
                    return (T) exist;
                var info = modelInfoMap.Get(id);
                var model = LoadModel<T>(info);
                if (modelMap.TryAdd(id, model))
                {
                    return model;
                }
            }
            return (T) modelMap.Get(id);
        }
    }

}
