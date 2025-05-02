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
        public const int TEAMSIZE = 3;

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

        public bool IsValidSlot(int slotIdx)
        {
            switch (teamType)
            {
                case TeamType.OneOneOne:
                    return true;
                case TeamType.OneTwo:
                    return slotIdx != 2;
                case TeamType.TwoOne:
                    return slotIdx != 1;
                case TeamType.Three:
                    return slotIdx == 0;
                default:
                    return false;
            }
        }

        public int EffectiveSize()
        {
            switch (teamType)
            {
                case TeamType.OneTwo:
                case TeamType.TwoOne:
                    return 2;
                case TeamType.Three:
                    return 1;
                default:
                    return 3;
            }
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