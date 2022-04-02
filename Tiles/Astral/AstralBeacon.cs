using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Astral
{
    public class AstralBeacon : ModTile
    {
        public const int Width = 5;
        public const int Height = 4;
        public static readonly Color FailColor = new Color(237, 93, 83);
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Astral Beacon");
            AddMapEntry(new Color(128, 128, 158), name);
            disableSmartCursor = true;
            minPick = 200;
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Utils.SelectRandom(Main.rand, ModContent.DustType<AstralBlue>(), ModContent.DustType<AstralOrange>());
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, Width * 16, Height * 16, ModContent.ItemType<AstralBeaconItem>());
        }

        public override bool NewRightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int left = i - tile.frameX / 18;
            int top = j - tile.frameY / 18;

            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<TitanHeart>()) &&
                !Main.LocalPlayer.HasItem(ModContent.ItemType<Starcore>()))
                return true;

            if (NPC.AnyNPCs(ModContent.NPCType<AstrumDeusHeadSpectral>()) || BossRushEvent.BossRushActive)
                return true;

            if (CalamityUtils.CountProjectiles(ModContent.ProjectileType<DeusRitualDrama>()) > 0)
                return true;

            bool usingStarcore = Main.LocalPlayer.HasItem(ModContent.ItemType<Starcore>());

            if (Main.dayTime)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DeusAltarRejectNightText", FailColor);
                return false;
            }

            Vector2 ritualSpawnPosition = new Vector2(left + Width / 2, top).ToWorldCoordinates();
            ritualSpawnPosition += new Vector2(0f, -24f);

            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstralBeaconUse"), ritualSpawnPosition);
            Projectile.NewProjectile(ritualSpawnPosition, Vector2.Zero, ModContent.ProjectileType<DeusRitualDrama>(), 0, 0f, Main.myPlayer, 0f, usingStarcore.ToInt());

            if (!usingStarcore)
                Main.LocalPlayer.ConsumeItem(ModContent.ItemType<TitanHeart>(), true);

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.showItemIcon2 = ModContent.ItemType<TitanHeart>();
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<Starcore>()))
                Main.LocalPlayer.showItemIcon2 = ModContent.ItemType<Starcore>();
            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.showItemIcon = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            Main.LocalPlayer.showItemIcon2 = ModContent.ItemType<TitanHeart>();
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<Starcore>()))
                Main.LocalPlayer.showItemIcon2 = ModContent.ItemType<Starcore>();
            Main.LocalPlayer.noThrow = 2;
            Main.LocalPlayer.showItemIcon = true;
        }
    }
}
