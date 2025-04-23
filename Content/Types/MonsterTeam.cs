using Monstermon.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Monstermon.Content.Types
{
    public class MonsterTeam : TagSerializable
    {
        public static readonly System.Func<TagCompound, MonsterTeam> DESERIALIZER = Load;
        public Item[] slots = [new(), new(), new(),];
        public TeamType teamType = TeamType.OneOneOne;

        public bool AddMonster(CapturedMonster item, int slotIdx, bool force_swap = false)
        {
            // For now, all monsters take up one slot
            if (slots[slotIdx] is null)
            {
                slots[slotIdx] = item.Item;
                return true;
            }
            return false;
        }

        public static MonsterTeam Load(TagCompound tag)
        {
            var team = new MonsterTeam();
            team.teamType = (TeamType)tag.GetInt("type");
            if (tag.TryGet("slots", out Item[] n_slots))
                team.slots = n_slots;

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