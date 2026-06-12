using System.Linq;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SarosPossession : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public static SoundStyle FiringSound => new SoundStyle("CalamityMod/Sounds/Item/Summon/SarosFiring") with { MaxInstances = 1 ,Volume = 0.3f, pitchVariance = 0.05f, pitch = 0.5f };
        public static SoundStyle SpawnSound => new SoundStyle("CalamityMod/Sounds/Item/Summon/SarosSpawn");
        public static SoundStyle LoopSound => SoundID.DD2_BetsyFlameBreath with { Volume = 0.2f };
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 48;
            Item.damage = 66;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1.15f;
            Item.UseSound = null;
            Item.buffType = ModContent.BuffType<SarosPossessionBuff>();
            Item.shoot = ModContent.ProjectileType<SarosAura>();
            Item.DamageType = DamageClass.Summon;
            Item.channel = true;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SarosEclipseBeam>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] > 0)
            {
                var p = Main.projectile.First(x => x.active && x.type == type && x.owner == player.whoAmI);
                p.ai[0]++;
                p.netUpdate = true;

                SoundEngine.PlaySound(SpawnSound with { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, pitchVariance = 0.05f, }, player.Center);
                return false;
            } else
            {
                player.channel = false;
            }
            player.AddBuff(Item.buffType, 2);

            SoundEngine.PlaySound(SpawnSound with { MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, pitchVariance = 0.05f, }, player.Center);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sirius>().
                AddIngredient<AuricBar>(5).
                AddIngredient<DarksunFragment>(15).
                AddTile<CosmicAnvil>().
                Register();
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int currentCooldown = Main.LocalPlayer.Calamity().sarosEclipseBeamUsage;
            float fill = 1 - currentCooldown / 300f;

            if (fill >= 1)
               return;


            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            float barScale = 1/(32f/TextureAssets.Item[Type].Width()) * 0.9f;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            float yOffset = 0f;
            Vector2 drawPos = position + new Vector2(0, (float)TextureAssets.Item[Type].Height() * 0.5f) * scale;
            Rectangle frameCrop = new Rectangle(0, 0, (int)((fill) * barFG.Width), barFG.Height);
            Color colorBG = Color.Black;
            Color colorFG = Color.Lerp(Color.OrangeRed, new Color(238, 226, 153), fill);

            spriteBatch.Draw(barBG, drawPos, null, colorBG, 0f, barOrigin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, colorFG, 0f, barOrigin, scale * barScale, 0f, 0f);
        }
    }
}
