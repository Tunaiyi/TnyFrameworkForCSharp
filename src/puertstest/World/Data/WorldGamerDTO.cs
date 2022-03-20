using System.Collections.Generic;
using System.Linq;
using GF.Base;
using GF.Game;
namespace puertstest.World.Data
{
    public class WorldGamerDTO : WorldGamerData
    {
        private long id;
        private Dictionary<string, GameParam> extraMap;

        long WorldGamerData.Id => id;
        public long PlayerId { get; }

        int WorldGamerData.ModelId => GameItemType.GAMER.Id;

        int WorldGamerData.HP => 20000;

        int WorldGamerData.MaxHP => 20000;

        int WorldGamerData.MP => 100;

        int WorldGamerData.MaxMP => 100;

        int WorldGamerData.Speed => 7;

        int WorldGamerData.Attack => 50;

        public IList<WorldSkillData> Skills { get; }
        public IList<int> SelectedSkillModelIds { get; }
        public IList<long> selectedSkillIds { get; }

        public IList<GameParam> Extras => extraMap.Values.ToList();
        
        public WorldGamerDTO(long id)
        {
            this.id = id;

            Skills = new List<WorldSkillData>();
            Skills.Add(new WorldSkillDTO(GameItemType.SKILL.Id));
            Skills.Add(new WorldSkillDTO(GameItemType.SKILL.Id + 1));
            Skills.Add(new WorldSkillDTO(GameItemType.SKILL.Id + 2));
            Skills.Add(new WorldSkillDTO(GameItemType.SKILL.Id + 3));
            Skills.Add(new WorldSkillDTO(GameItemType.SKILL.Id + 4));

            selectedSkillIds = new List<long>();
            foreach (var skill in Skills)
            {
                selectedSkillIds.Add(skill.Id);
            }

            extraMap = new Dictionary<string, GameParam>();
            extraMap.Add("LockStepGamerId", GameParam.Create("LockStepGamerId", (int)id));
        }


        public GameParam GetExtra(string key)
        {
            return extraMap.Get(key);
        }
    }
}
