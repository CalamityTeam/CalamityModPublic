using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ArkoftheCosmos : ModItem
    {
        public float Combo = 0f;
        public float Charge = 0f;
        public override bool CloneNewInstances => true;

        public static float NeedleDamageMultiplier = 0.7f; //Damage on the non-homing needle projectile
        public static float MaxThrowReach = 620;
        public static float snapDamageMultiplier = 1.3f; //Extra damage from making the scissors snap

        public static float chargeDamageMultiplier = 1.3f; //Extra damage from charge
        public static float chainDamageMultiplier = 0.1f;

        public static int DashIframes = 10;
        public static float SlashBoltsDamageMultiplier = 0.2f;
        public static float SnapBoltsDamageMultiplier = 0.1f;

        public static float blastDamageMultiplier = 0.5f; //Damage multiplier applied ontop of the charge damage multiplier mutliplied by the amount of charges consumed. So if you consume 5 charges, the blast will get multiplied by 5 times the damage multiplier
        public static float blastFalloffSpeed = 0.1f; //How much the blast damage falls off as you hit more and more targets 
        public static float blastFalloffStrenght = 0.75f; //Value between 0 and 1 that determines how much falloff increases affect the damage : Closer to 0 = damage falls off less intensely, closer to 1 : damage falls off way harder

        public static float SwirlBoltAmount = 7f; //The amount of cosmic bolts produced during hte swirl attack
        public static float SwirlBoltDamageMultiplier = 1.5f; //This is the damage multiplier for ALL THE BOLTS: Aka, said damage multiplier is divided by the amount of bolts in a swirl and the full damage multiplier is gotten if you hit all the bolts

        const string ComboTooltip = "Performs a combo of swings, alternating between narrow and wide swings and throwing the blade out every 5 swings\n" +
                "The thrown blade is held in place by constellations and will follow your cursor\n" +
                "Releasing the mouse while the blade is out will throw the second half towards it, making the scissors snap and explode into stars";

        const string ParryTooltip = "Using RMB will snip out the scissor blades in front of you.n" +
                "Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 200 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, letting you use both blades at once";

        const string BlastTooltip = "Using RMB and pressing up while the Ark is empowered will throw the blades in front of you to provoke a Big Rip in spacetime, using up all your charges in the process\n" +
                "If more than half the charges were used up, you can dash across the rip by keeping the up key pressed down";


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Cosmos");
            Tooltip.SetDefault("This line gets set in ModifyTooltips\n" +
                "This line also gets set in ModifyTooltips\n" +
                "This line also gets set in ModifyTooltips\n" +
                "The physical culmination of your journey, capable to rend gods asunder");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var comboTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.mod == "Terraria");
            comboTooltip.text = ComboTooltip;
            comboTooltip.overrideColor = Color.Lerp(Color.Gold, Color.Goldenrod, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);

            var parryTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip1" && x.mod == "Terraria");
            parryTooltip.text = ParryTooltip;
            parryTooltip.overrideColor = Color.Lerp(Color.Cyan, Color.DeepSkyBlue, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.75f);

            var blastTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip2" && x.mod == "Terraria");
            blastTooltip.text = BlastTooltip;
            blastTooltip.overrideColor = Color.Lerp(Color.HotPink, Color.Crimson, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.625f);
        }

        public override void SetDefaults()
        {
            item.width = item.height = 136;
            item.damage = 2222;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9.5f;
            item.UseSound = null;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 28f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 15;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;

            if (CanUseItem(player) && Combo != 4)
                item.channel = false;

            if (Combo == 4)
                item.channel = true;
        }
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ArkoftheCosmosSwungBlade>());
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = new Vector2(speedX, speedY).ToRotation();
                    Projectile.NewProjectile(player.Center + angle.ToRotationVector2() * 90f, new Vector2(speedX, speedY), ProjectileType<ArkoftheCosmosBlast>(), (int)(damage * Charge * chargeDamageMultiplier * blastDamageMultiplier), 0, player.whoAmI, Charge);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>() || n.type == ProjectileType<ArkoftheCosmosParryHoldout>())))
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheCosmosParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);

            float scissorState = Combo == 4 ? 2 : Combo % 2;

            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheCosmosSwungBlade>(), damage, knockBack, player.whoAmI, scissorState, Charge);


            //Shoot projectiles 
            if (scissorState != 2)
            {
                Vector2 throwVector = new Vector2(speedX, speedY);
                Projectile.NewProjectile(player.Center + Utils.SafeNormalize(throwVector, Vector2.Zero) * 20, new Vector2(speedX, speedY) * 1.4f, ProjectileType<RendingNeedle>(), (int)(damage * NeedleDamageMultiplier), knockBack, player.whoAmI);
            }

            Combo += 1;
            if (Combo > 4)
                Combo = 0;

            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<FourSeasonsGalaxia>());
            recipe.AddIngredient(ItemType<ArkoftheElements>());
            recipe.AddIngredient(ItemType<AuricBar>(), 5);
            recipe.AddTile(TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            (clone as ArkoftheCosmos).Charge = (item.modItem as ArkoftheCosmos).Charge;

            return clone;
        }
        public override ModItem Clone()
        {
            var clone = base.Clone();

            (clone as ArkoftheCosmos).Charge = Charge;

            return clone;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Charge);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            Charge = reader.ReadInt32();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D handleTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheCosmosHandle");
            Texture2D bladeTexture = GetTexture("CalamityMod/Items/Weapons/Melee/ArkoftheCosmosGlow");

            float bladeOpacity = (Charge > 0) ? 1f : MathHelper.Clamp((float)Math.Sin(Main.GlobalTime % MathHelper.Pi) * 2f, 0, 1) * 0.7f + 0.3f;

            spriteBatch.Draw(handleTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f); //Make the back scissor slightly transparent if the ark isnt charged
            spriteBatch.Draw(bladeTexture, position, null, drawColor * bladeOpacity, 0f, origin, scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Charge <= 0)
                return;

            var barBG = GetTexture("CalamityMod/ExtraTextures/GenericBarBack");
            var barFG = GetTexture("CalamityMod/ExtraTextures/GenericBarFront");

            float barScale = 3f;

            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 8) * scale + Vector2.UnitX * (frame.Width - barBG.Width * barScale) * scale * 0.5f;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(Charge / 10f * barFG.Width), barFG.Height);
            Color color = Main.hslToRgb((Main.GlobalTime * 0.6f) % 1, 1, 0.75f + (float)Math.Sin(Main.GlobalTime * 3f) * 0.1f);

            spriteBatch.Draw(barBG, drawPos, null, color, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, color * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }
    }
}
