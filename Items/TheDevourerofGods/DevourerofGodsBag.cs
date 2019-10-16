using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class DevourerofGodsBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DevourerofGodsHeadS>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(18f, 20f); //18, 17
            spriteBatch.Draw(mod.GetTexture("Items/TheDevourerofGods/DevourerofGodsBagGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<CosmiliteBar>(), 30, 39);
            DropHelper.DropItem(player, ModContent.ItemType<CosmiliteBrick>(), 200, 320);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<Excelsus>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<EradicatorMelee>(), 3);
            float dischargeChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<CosmicDischarge>(), CalamityWorld.revenge, dischargeChance);
            DropHelper.DropItemChance(player, ModContent.ItemType<TheObliterator>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Deathwind>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Skullmasher>(), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItemChance(player, ModContent.ItemType<Norfleet>(), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItemChance(player, ModContent.ItemType<DeathhailStaff>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<StaffoftheMechworm>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Eradicator>(), 3);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<NebulousCore>());
            bool vodka = player.Calamity().fabsolVodka;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Fabsol>(), CalamityWorld.revenge && vodka);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<DevourerofGodsMask>(), 7);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CosmicPlushie>(), CalamityWorld.death && player.difficulty == 2);
        }
    }
}
