using System.Collections.Generic;
using GF.Game;
namespace puertstest.World.Data
{
    public class WorldOriginalDTO : WorldOriginalData
    {
        public int ModelId => GameItemType.WORLD_MAP.Id;

        public long Id { get; }

        public WorldType Type { get; }

        public int SelfId { get; }

        public IList<WorldGamerData> Games { get; }


        public WorldOriginalDTO()
        {
        }


        public WorldOriginalDTO(long id, WorldType type, int selfId, IList<long> gamerIds)
        {
            Id = id;
            Type = type;
            SelfId = selfId;
            Games = new List<WorldGamerData>();
            foreach (var gamerId in gamerIds)
            {
                Games.Add(new WorldGamerDTO(gamerId));
            }
        }


        public IList<WorldGamerData> GetGamers()
        {
            return Games;
        }
    }
}
