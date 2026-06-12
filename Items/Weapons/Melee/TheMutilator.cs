using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheMutilator : BaseSwordHoldoutItem, ILocalizedModType
    {
        public static int MaximumCharge = 7;
        public int Charge = 0;

        public int DecayTimer = 0;
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<MutilatorSwordProj>();
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Laceration>()];
        }
        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 90;
            Item.damage = 1005;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 8f;
            Item.shootSpeed = 10f;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            base.SetDefaults();
        }


        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage *= 1 + Charge / 7f;
        }
        public override void UpdateInventory(Player player)
        {
            if (DecayTimer > 0) DecayTimer--;
            else
            {
                if (Charge > 0)
                {
                    Charge--;
                    DecayTimer = 60;
                }
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float fill = Charge / (float)MaximumCharge;
            if (fill <= 0)
                return;

            float barScale = 3f;

            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            float yOffset = 44f;
            Vector2 drawPos = position + Vector2.UnitY * scale * (frame.Height - yOffset);
            Rectangle frameCrop = new Rectangle(0, 0, (int)((fill) * barFG.Width), barFG.Height);
            Color colorBG = Color.Crimson;
            Color colorFG = Color.Lerp(Color.OrangeRed, Color.DarkOrange, fill);

            spriteBatch.Draw(barBG, drawPos, null, colorBG, 0f, barOrigin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, colorFG, 0f, barOrigin, scale * barScale, 0f, 0f);
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
