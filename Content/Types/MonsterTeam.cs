using Monstermon.Content.Items;
using Terraria.ModLoader.IO;

namespace Monstermon.Content.Types
{
    public class MonsterTeam : TagSerializable
    {
        public static readonly System.Func<TagCompound, MonsterTeam> DESERIALIZER = Load;
        public CapturedMonster?[] slots;
        public TeamType teamType;

        public MonsterTeam()
        {
            slots = new CapturedMonster?[3];
            teamType = TeamType.OneOneOne;
        }

        public bool AddMonster(CapturedMonster item, int slotIdx, bool force_swap = false)
        {
            // For now, all monsters take up one slot
            if (slots[slotIdx] is null)
            {
                slots[slotIdx] = item;
                return true;
            }
            return false;
        }

        public static MonsterTeam Load(TagCompound tag)
        {
            var team = new MonsterTeam();
            team.teamType = (TeamType)tag.GetInt("type");
            return team;
        }

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["slots"] = slots,
                ["type"] = (int)teamType
            };
        }
    }

    public enum TeamType
    {
        OneOneOne,
        OneTwo,
        TwoOne,
        Three
    }
}