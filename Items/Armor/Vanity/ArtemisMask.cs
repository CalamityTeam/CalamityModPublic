using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ArtemisMask : ModItem, IExtendedHat
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Artemis Mask");

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }

        public string ExtensionTexture => "CalamityMod/Items/Armor/Vanity/ArtemisMask_Extra";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(drawInfo.drawPlayer.direction == 1f ? -6f : 0, 0f);
    }
}
