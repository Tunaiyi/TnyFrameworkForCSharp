using GF.Game;
namespace puertstest.World.Data
{
    public class WorldSkillDTO : WorldSkillData
    {
        public long Id { get; set; }

        public int ModelId { get; set; }


        public WorldSkillDTO()
        {
        }


        public WorldSkillDTO(int modelId)
        {
            Id = modelId;
            ModelId = modelId;
        }
    }
}
