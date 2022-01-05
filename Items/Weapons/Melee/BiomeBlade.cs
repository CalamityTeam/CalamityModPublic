using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Terraria.Graphics.Shaders;
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
    public class BiomeBlade : ModItem
    {
        public enum Attunement : byte { Default, Hot, Cold, Tropical, Evil }
        public Attunement? mainAttunement = null;
        public Attunement? secondaryAttunement = null;
        public int Combo = 0;
        public int CanLunge = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Biome Blade"); //Broken Ecoliburn lmfao. Tbh a proper name instead of just "biome blade" may be neat given the importance of the sword
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "Use RMB while standing still on the ground to attune the weapon to the powers of the surrounding biome\n" +
                               "Using RMB while moving or in the air switches between the current attunement and an extra stored one\n" +
                               "Main attunement : None\n" +
                               "Secondary attunement: None\n"); //Theres potential for flavor text as well but im not a writer
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            foreach (TooltipLine l in list) //I gave the attunement silly pseudo-fantasy names, ideas appreciated.
            {
                if (l.text.StartsWith("FUNCTION_DESC"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = info.color;
                    l.text = info.function_description;
                }

                if (l.text.StartsWith("Main attunement"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = info.color;
                    l.text = "Main Attumenent : ["+ info.name + "]";
                }

                if (l.text.StartsWith("Secondary attunement"))
                {
                    AttunementInfo info = GetAttunementInfo(secondaryAttunement);
                    l.overrideColor = Color.Lerp(info.color, Color.Gray, 0.5f);
                    l.text = "Secondary Attumenent : [" + info.name + "]";
                } 
            }
        }

        internal struct AttunementInfo
        {
            public string name;
            public string function_description;
            public Color color;
        }

        internal AttunementInfo GetAttunementInfo(Attunement? attunement)
        {
            AttunementInfo AttunementInfo = new AttunementInfo();

            switch (attunement)
            {
                case Attunement.Default:
                    AttunementInfo.name = "Pure Clarity";
                    AttunementInfo.function_description = "Fires a weak projectile that crushes enemy defenses";
                    AttunementInfo.color = new Color(171, 180, 73);
                    break;
                case Attunement.Hot:
                    AttunementInfo.name = "Arid Grandeur";
                    AttunementInfo.function_description = "Conjures searing blades in front of you that get larger and stronger the more you hit enemies. The blades can also be used to bounce off tiles when in the air";
                    AttunementInfo.color = new Color(238, 156, 73);
                    break;
                case Attunement.Cold:
                    AttunementInfo.name = "Biting Embrace";
                    AttunementInfo.function_description = "Perform a 3 strike combo with a glacial blade. The final strike freezes foes for a split second";
                    AttunementInfo.color = new Color(165, 235, 235);
                    break;
                case Attunement.Tropical:
                    AttunementInfo.name = "Grovetender's Touch";
                    AttunementInfo.function_description = "Throw out the blade using a vine whip. Striking enemies with the tip of the whip as it cracks guarantees a critical hit. The whip will also propel you towards struck tiles";
                    AttunementInfo.color = new Color(162, 200, 85);
                    break;
                case Attunement.Evil:
                    AttunementInfo.name = "Decay's Retort";
                    AttunementInfo.function_description = "Lunge forward using a ghostly rapier projection that leeches life off any struck foes. You also get bounced away from hit targets";
                    AttunementInfo.color = new Color(211, 64, 147);
                    break;
                default:
                    AttunementInfo.name = "None";
                    AttunementInfo.function_description = "Does nothing... yet";
                    AttunementInfo.color = new Color(163, 163, 163);
                    break;
            }

            return AttunementInfo;
        }

        public override void SetDefaults()
        {
            item.width = item.height = 36;
            item.damage = 55;
            item.melee = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shoot = ProjectileID.PurificationPowder; 
            item.knockBack = 5f;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyWoodenSword");
            recipe.AddIngredient(ItemID.DirtBlock, 50);
            recipe.AddIngredient(ItemID.StoneBlock, 50);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddIngredient(ItemType<VictoryShard>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item) //need to add shit to properly save those
        {
            var clone = base.Clone(item);

            if (Main.mouseItem.type == ItemType<BiomeBlade>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);

            (clone as BiomeBlade).mainAttunement = (item.modItem as BiomeBlade).mainAttunement;
            (clone as BiomeBlade).secondaryAttunement = (item.modItem as BiomeBlade).secondaryAttunement;

            //As funny as a Broken Broken Biome Blade would be, its also quite funny to make it turn into that. This is only done for a new instance of the item since the goblin tinkerer changes prevent it from happening through reforging
            if (clone.item.prefix == PrefixID.Broken)
            {
                clone.item.Prefix(PrefixID.Legendary);
                clone.item.prefix = PrefixID.Legendary;
            }

            return clone;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)mainAttunement);
            writer.Write((byte)secondaryAttunement);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = (Attunement?)reader.ReadByte();
            secondaryAttunement = (Attunement?)reader.ReadByte();
        }

        public override void HoldItem(Player player)
        {

            if (player.velocity.Y == 0) //Reset the lunge ability on ground contact
                CanLunge = 1;

            //Change the swords function based on its attunement
            switch (mainAttunement)
            {
                case Attunement.Default:
                    item.channel = false;
                    item.noUseGraphic = false;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileType<PurityProjection>();
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;
                    item.noMelee = false;

                    Combo = 0;
                    break;
                case Attunement.Hot:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<AridGrandeur>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    break;
                case Attunement.Cold:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<BitingEmbrace>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;
                    break;
                case Attunement.Tropical:
                    item.channel = false;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileType<GrovetendersTouch>();
                    item.shootSpeed = 30;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    break;
                case Attunement.Evil:
                    item.channel = false;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.Stabbing;
                    item.shoot = ProjectileType<DecaysRetort>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    break;
                default:
                    item.noUseGraphic = false;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileID.PurificationPowder;
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;

                    Combo = 0;
                    break;
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<BitingEmbrace>() || n.type == ProjectileType<GrovetendersTouch>() || n.type == ProjectileType<AridGrandeur>()));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BrokenBiomeBladeVisuals>() && n.owner == player.whoAmI))
                    return false;

                if (!player.StandingStill()) //Swap attunements
                {
                    if (secondaryAttunement == null) //Don't let the player swap to an empty attunement
                        return false;

                    Projectile.NewProjectile(player.position, Vector2.Zero, ProjectileType<BrokenBiomeBladeVisuals>(), 0, 0, player.whoAmI, 0f);
                    Attunement? temporaryAttunementStorage = mainAttunement;
                    mainAttunement = secondaryAttunement;
                    secondaryAttunement = temporaryAttunementStorage;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Main.PlaySound(SoundID.DD2_DarkMageHealImpact);
                    }
                }

                else //Chargeup
                {
                    if (player.mount.Active)
                        return false;
                    Vector2 displace = new Vector2((player.direction >= 0 ? 2 : -1) * 20f, 0);
                    Projectile.NewProjectile(player.position + displace, Vector2.Zero, ProjectileType<BrokenBiomeBladeVisuals>(), 0, 0, player.whoAmI, 1f);
                    if (player.whoAmI == Main.myPlayer)
                        Main.PlaySound(SoundID.DD2_DarkMageHealImpact);
                }
                return false;
            }

            if (mainAttunement == null)
                return false;

            switch (mainAttunement)
            {
                case Attunement.Cold:
                    switch (Combo)
                    {
                        case 0:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 0, 15);
                            break;

                        case 1:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 1, 20);
                            break;

                        case 2:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 2, 50);
                            break;
                    }
                    Combo++;
                    if (Combo > 2)
                        Combo = 0;
                    return false;

                case Attunement.Evil:
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<DecaysRetort>(), damage*2, knockBack, player.whoAmI, 26, (float) CanLunge);
                    CanLunge = 0;
                    return false;

                default:
                    return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
            }
        }

            


        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.DarkViolet, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D itemTexture = Main.itemTexture[item.type];
            Rectangle itemFrame = (Main.itemAnimations[item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[item.type].GetFrame(itemTexture);

            if (mainAttunement == null)
                return true;

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;

            switch (mainAttunement)
            {
                case Attunement.Default:
                    BiomeEnergyParticles.EdgeColor = new Color(117, 126, 72);
                    BiomeEnergyParticles.CenterColor = new Color(200, 184, 136);
                    break;
                case Attunement.Hot:
                    BiomeEnergyParticles.EdgeColor = new Color(137, 32, 0);
                    BiomeEnergyParticles.CenterColor = new Color(209, 154, 0);
                    break;
                case Attunement.Cold:
                    BiomeEnergyParticles.EdgeColor = new Color(165, 235, 235);
                    BiomeEnergyParticles.CenterColor = new Color(58, 110, 141);
                    break;
                case Attunement.Tropical:
                    BiomeEnergyParticles.EdgeColor = new Color(53, 112, 4);
                    BiomeEnergyParticles.CenterColor = new Color(131, 173, 39);
                    break;
                case Attunement.Evil:
                    BiomeEnergyParticles.EdgeColor = new Color(112, 4, 35);
                    BiomeEnergyParticles.CenterColor = new Color(195, 42, 200);
                    break;
            }
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTime * 3f) * 2f * (float)Math.Sin(Main.GlobalTime);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(itemTexture, position + displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(default, default);

            return true;
        }


    }

    public class BrokenBiomeBladeVisuals : ModProjectile //Visuals
    {
        private Player Owner => Main.player[projectile.owner];
        public ref float AnimationMode => ref projectile.ai[0];
        private BezierCurve bezierCurve;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Biome Blade");
        }
        public override string Texture => "CalamityMod/Items/Weapons/Melee/BiomeBlade";
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public override void AI()
        {
            if (Owner.HeldItem.type != ItemType<BiomeBlade>())
            {
                projectile.timeLeft = 0;
                return;
            }

            if (AnimationMode == 1f)
            {
                if (!Owner.StandingStill())
                {
                    projectile.timeLeft = 0;
                    return;
                }
                if (projectile.timeLeft == 30)
                {
                    List<Vector2> bezierPoints = new List<Vector2>() { projectile.position, projectile.position - Vector2.UnitY * 16f, projectile.position + Vector2.UnitY * 56f, projectile.position + Vector2.UnitY * 42f };
                    bezierCurve = new BezierCurve(bezierPoints.ToArray());
                }

                projectile.position = bezierCurve.Evaluate(1f - projectile.timeLeft / 30f);
                //projectile.rotation = MathHelper.Lerp(MathHelper.PiOver4, -(MathHelper.PiOver2 + MathHelper.PiOver4), 1f - MathHelper.Clamp((projectile.timeLeft / 10), 0f, 1f));
                //projectile.rotation = MathHelper.PiOver4 + MathHelper.PiOver2;
                projectile.rotation = Utils.AngleLerp(MathHelper.PiOver4 + MathHelper.PiOver2, -MathHelper.PiOver4, MathHelper.Clamp(((projectile.timeLeft - 20f) / 10f), 0f, 1f));

                if (projectile.timeLeft == 2)
                {
                    bool jungle = Owner.ZoneJungle;
                    bool ocean = Owner.ZoneBeach;
                    bool snow = Owner.ZoneSnow;
                    bool evil = Owner.ZoneCorrupt || Owner.ZoneCrimson;
                    bool desert = Owner.ZoneDesert;
                    bool hell = Owner.ZoneUnderworldHeight;

                    BiomeBlade.Attunement attunement = BiomeBlade.Attunement.Default;

                    if (jungle || ocean)
                    {
                        attunement = BiomeBlade.Attunement.Tropical;
                    }
                    if (desert || hell)
                    {
                        attunement = BiomeBlade.Attunement.Hot;
                    }
                    if (snow)
                    {
                        attunement = BiomeBlade.Attunement.Cold;
                    }
                    if (evil)
                    {
                        attunement = BiomeBlade.Attunement.Evil;
                    }

                    if ((Owner.HeldItem.modItem as BiomeBlade).mainAttunement == attunement)
                    {
                        Main.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                        return;
                    }
                    Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
                    (Owner.HeldItem.modItem as BiomeBlade).secondaryAttunement = (Owner.HeldItem.modItem as BiomeBlade).mainAttunement;
                    (Owner.HeldItem.modItem as BiomeBlade).mainAttunement = attunement;

                    //Lots of particles!!! yay!! visuals!!
                }
            }

            //Cool particles
        }

        public override void Kill(int timeLeft)
        {
            //Cool particles
        }
    }

}


