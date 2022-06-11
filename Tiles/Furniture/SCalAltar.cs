using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Audio;

namespace CalamityMod.Tiles.Furniture
{
    public class SCalAltar : ModTile
    {
        public const int Width = 4;
        public const int Height = 3;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Altar");
            AddMapEntry(new Color(43, 19, 42), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            // Fire dust.
            type = 6;
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, Width * 16, Height * 16, ModContent.ItemType<AltarOfTheAccursedItem>());
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<AshesofCalamity>()) &&
                !Main.LocalPlayer.HasItem(ModContent.ItemType<EyeofExtinction>()))
            {
                return true;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) || BossRushEvent.BossRushActive)
                return true;

            if (CalamityUtils.CountProjectiles(ModContent.ProjectileType<SCalRitualDrama>()) > 0)
                return true;

            bool usingSpecialItem = Main.LocalPlayer.HasItem(ModContent.ItemType<EyeofExtinction>());

            Vector2 ritualSpawnPosition = new Vector2(left + Width / 2, top).ToWorldCoordinates();
            ritualSpawnPosition += new Vector2(-10f, -24f);

            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, ritualSpawnPosition);
            Projectile.NewProjectile(new EntitySource_WorldEvent(), ritualSpawnPosition, Vector2.Zero, ModContent.ProjectileType<SCalRitualDrama>(), 0, 0f, Main.myPlayer);

            if (!usingSpecialItem)
                Main.LocalPlayer.ConsumeItem(ModContent.ItemType<AshesofCalamity>(), true);

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<EyeofExtinction>()))
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<EyeofExtinction>();
            else
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AshesofCalamity>();

            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<EyeofExtinction>()))
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<EyeofExtinction>();
            else
                Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AshesofCalamity>();

            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.cursorItemIconEnabled = true;
        }
    }
}
