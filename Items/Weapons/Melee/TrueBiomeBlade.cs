using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static CalamityMod.CalamityUtils;
using static CalamityMod.Items.Weapons.Melee.TrueBiomeBlade;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueBiomeBlade : ModItem
    {
        public enum Attunement : byte { Default, Hot, Cold, Tropical, Evil, Holy, Astral, Marine }
        public Attunement? mainAttunement = null;
        public Attunement? secondaryAttunement = null;
        public int Combo = 0;
        public int StoredLunges = 2;
        public int PowerLungeCounter = 0;
        public bool strongLunge = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biome Blade");
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "FUNCTION_EXTRA\n" +
                               "Use RMB while standing still on the ground to attune the weapon to the powers of the surrounding biome\n" +
                               "Using RMB otherwise switches between the current attunement and an extra stored one\n" +
                               "Main attunement : None\n" +
                               "Secondary attunement: None\n"); //Theres potential for flavor text as well but im not a writer
        }

        #region tooltip editing

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            foreach (TooltipLine l in list)
            {
                if (l.text.StartsWith("FUNCTION_DESC"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = info.color;
                    l.text = info.function_description;
                }

                if (l.text.StartsWith("FUNCTION_EXTRA"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = info.color;
                    l.text = info.function_extra;
                }

                if (l.text.StartsWith("Main attunement"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = info.color;
                    l.text = "Main Attumenent : [" + info.name + "]";
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
            public string function_extra;
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
                    AttunementInfo.function_extra = "Landing true melee hits places a sigil of purity over the enemy. Your projectiles home onto the marked foes"; 
                    AttunementInfo.color = new Color(171, 180, 73);
                    break;
                case Attunement.Hot:
                    AttunementInfo.name = "Arid Grandeur";
                    AttunementInfo.function_description = "Conjures searing blades in front of you that get larger and stronger the more you hit enemies. The blades can also be used to bounce off tiles when in the air";
                    AttunementInfo.function_extra = "Releasing LMB after charging up the blades sends them out flying to shred your foes";
                    AttunementInfo.color = new Color(238, 156, 73);
                    break;
                case Attunement.Cold:
                    AttunementInfo.name = "Biting Embrace";
                    AttunementInfo.function_description = "Perform a 3 strike combo with a glacial blade. The final strike freezes foes for a split second";
                    AttunementInfo.function_extra = "Lingering glacial mist is released on each swing, dealing damage to enemies wandering into it";
                    AttunementInfo.color = new Color(165, 235, 235);
                    break;
                case Attunement.Tropical:
                    AttunementInfo.name = "Grovetender's Touch";
                    AttunementInfo.function_description = "Throw out the blade using a vine whip. Striking enemies with the tip of the whip as it cracks guarantees a critical hit. The whip will also propel you towards struck tiles";
                    AttunementInfo.function_extra = "Critical strikes sprout up extra vines that can hit up to three targets";
                    AttunementInfo.color = new Color(162, 200, 85);
                    break;
                case Attunement.Evil:
                    AttunementInfo.name = "Decay's Retort";
                    AttunementInfo.function_description = "Lunge forward using a ghostly rapier projection that leeches life off any struck foes. You also get bounced away from hit targets";
                    AttunementInfo.function_extra = "Lining up three strikes without touching the ground empowers the next strike into a powerful dash thrust ";
                    AttunementInfo.color = new Color(211, 64, 147);
                    break;
                case Attunement.Holy:
                    AttunementInfo.name = "Heaven's Might";
                    AttunementInfo.function_description = "Hold LMB to swing the sword around you, powering up as it spins";
                    AttunementInfo.function_extra = "Releasing LMB will throw the sword in front of you, flying a short distance away before coming back";
                    AttunementInfo.color = new Color(220, 143, 255);
                    break;
                case Attunement.Astral:
                    AttunementInfo.name = "Their Abhorrence";
                    AttunementInfo.function_description = "Hold LMB to charge up a star-piercing sword thrust, and release to unleash the devastating blow";
                    AttunementInfo.function_extra = "Striking the ground will create an impact so powerful, massive otherwordly monoliths will rise up from the earth";
                    AttunementInfo.color = new Color(91, 73, 196);
                    break;
                case Attunement.Marine:
                    AttunementInfo.name = "Gesture for the Drowned";
                    AttunementInfo.function_description = "Shoot out two water balls at your sides. On ground contact the water balls will turn into small waves that ride along the ground";
                    AttunementInfo.function_extra = "If used while submerged, or if the waves contact water, the waves will expand and move much faster";
                    AttunementInfo.color = new Color(98, 225, 255);
                    break;
                default:
                    AttunementInfo.name = "None";
                    AttunementInfo.function_description = "Does nothing... yet";
                    AttunementInfo.function_extra = "Repairing the blade did increase its attuning capacities";
                    AttunementInfo.color = new Color(163, 163, 163);
                    break;
            }

            return AttunementInfo;
        }

        #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 68;
            item.damage = 160;
            item.melee = true;
            item.useAnimation = 21;
            item.useTime = 21;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<BiomeBlade>());
            recipe.AddIngredient(ItemType<LivingShard>(), 5);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddIngredient(ItemType<DepthCells>(), 10); //Hilarious abyss mats moment. Shall we change that since the abyss gets a attunement anyways
            recipe.AddIngredient(ItemType<Lumenite>(), 10);
            recipe.AddIngredient(ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region Saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<TrueBiomeBlade>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);
            (clone as TrueBiomeBlade).mainAttunement = (item.modItem as TrueBiomeBlade).mainAttunement;
            (clone as TrueBiomeBlade).secondaryAttunement = (item.modItem as TrueBiomeBlade).secondaryAttunement;

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();
            (clone as TrueBiomeBlade).mainAttunement = mainAttunement;
            (clone as TrueBiomeBlade).secondaryAttunement = secondaryAttunement;

            return clone;
        }

        public override TagCompound Save()
        {
            int attunement1 = mainAttunement == null ? -1 : (int)mainAttunement;
            int attunement2 = secondaryAttunement == null ? -1 : (int)secondaryAttunement;
            TagCompound tag = new TagCompound
            {
                { "mainAttunement", attunement1 },
                { "secondaryAttunement", attunement2 }
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");
            int attunement2 = tag.GetInt("secondaryAttunement");
            if (attunement1 == -1)
                mainAttunement = null;
            else
                mainAttunement = (Attunement?)attunement1;

            if (attunement2 == -1 || (attunement1 == attunement2))
                secondaryAttunement = null;
            else
                secondaryAttunement = (Attunement?)attunement2;
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

        #endregion


        public override void HoldItem(Player player)
        {

            player.Calamity().rightClickListener = true;

            if (player.velocity.Y == 0) //reset the amount of lunges on ground contact
            {
                StoredLunges = 2;
                if (PowerLungeCounter != 3)
                    PowerLungeCounter = 0;
            }

            //Change the swords function based on its attunement
            switch (mainAttunement)
            {
                case Attunement.Default:
                    item.channel = false;
                    item.noUseGraphic = false;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileType<TruePurityProjection>();
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;
                    item.noMelee = false;

                    Combo = 0;
                    PowerLungeCounter = 0;
                    break;
                case Attunement.Hot:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<TrueAridGrandeur>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    PowerLungeCounter = 0;
                    break;
                case Attunement.Cold:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<TrueBitingEmbrace>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    PowerLungeCounter = 0;
                    break;
                case Attunement.Tropical:
                    item.channel = false;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileType<TrueGrovetendersTouch>();
                    item.shootSpeed = 30;
                    item.UseSound = null;
                    item.noMelee = true;

                    PowerLungeCounter = 0;
                    break;
                case Attunement.Evil:
                    item.channel = false;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.Stabbing;
                    item.shoot = ProjectileType<TrueDecaysRetort>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    break;
                case Attunement.Holy:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<HeavensMight>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    PowerLungeCounter = 0;
                    break;
                case Attunement.Astral:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.Stabbing;
                    item.shoot = ProjectileType<TheirAbhorrence>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    PowerLungeCounter = 0;
                    break;
                case Attunement.Marine:
                    item.channel = false;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.Stabbing;
                    //item.shoot = ProjectileType<GestureForTheDrowned>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    PowerLungeCounter = 0;
                    break;
                default:
                    item.noUseGraphic = false;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileID.PurificationPowder;
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;

                    Combo = 0;
                    PowerLungeCounter = 0;
                    break;
            }

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<MendedBiomeBladeVisuals>() && n.owner == player.whoAmI))
                    return;

                int x = (int)player.Center.X / 16;
                int y = (int)(player.position.Y + (float)player.height - 1f) / 16;
                Tile tileStandingOn = Main.tile[x, y + 1];

                bool mayAttune = player.StandingStill() && !player.mount.Active && tileStandingOn.IsTileSolidGround();
                Vector2 displace = new Vector2(18f, 0f);
                Projectile.NewProjectile(player.Top + displace, Vector2.Zero, ProjectileType<MendedBiomeBladeVisuals>(), 0, 0, player.whoAmI, mayAttune ? 0f : 1f);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<TrueBitingEmbrace>() || 
             n.type == ProjectileType<TrueGrovetendersTouch>() ||
             n.type == ProjectileType<TrueAridGrandeur>() ||
             n.type == ProjectileType<HeavensMight>() ||
             n.type == ProjectileType<TheirAbhorrence>()));  
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            strongLunge = false;

            if (mainAttunement == null)
                return false;

            switch (mainAttunement)
            {
                case Attunement.Cold:
                    switch (Combo)
                    {
                        case 0:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueBitingEmbrace>(), damage, knockBack, player.whoAmI, 0, 15);
                            break;

                        case 1:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueBitingEmbrace>(), damage, knockBack, player.whoAmI, 1, 20);
                            break;

                        case 2:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueBitingEmbrace>(), damage, knockBack, player.whoAmI, 2, 50);
                            break;
                    }
                    Combo++;
                    if (Combo > 2)
                        Combo = 0;
                    return false;

                case Attunement.Tropical:
                    Projectile whipProj = Projectile.NewProjectileDirect(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueGrovetendersTouch>(), damage, knockBack, player.whoAmI, 0, 0);
                    if (whipProj.modProjectile is TrueGrovetendersTouch whip)
                        whip.flipped = Combo == 0 ? 1 : -1;
                    Combo++;
                    if (Combo > 1)
                        Combo = 0;
                    return false;

                case Attunement.Evil:
                    bool powerLungeAvailable = false;
                    if (PowerLungeCounter == 3)
                    {
                        powerLungeAvailable = true;
                        PowerLungeCounter = 0;
                    }
                    Projectile proj = Projectile.NewProjectileDirect(player.Center, new Vector2(speedX, speedY), ProjectileType<TrueDecaysRetort>(), damage * 2, knockBack, player.whoAmI, 26f, StoredLunges > 0 ? 1f : 0f);
                    if (proj.modProjectile is TrueDecaysRetort rapier)
                        rapier.ChargedUp = powerLungeAvailable;
                    StoredLunges --;
                    if (StoredLunges < 0)
                        StoredLunges = 0;
                    return false;

                default:
                    return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
            }
        }

        //This is only used for the purity sigil effect of the default attunement
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == player.whoAmI)
                {
                    //Reset the timeleft on the sigil & give it its new target (or the same, it doesnt matter really.
                    proj.ai[0] = target.whoAmI;
                    proj.timeLeft = 600;
                    return;
                }
            }
            Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<PurityProjectionSigil>(), 0, 0, player.whoAmI, target.whoAmI);
        }

        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.White, Color.White, 0.04f, 20f);
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
                case Attunement.Holy:
                case Attunement.Astral:
                    BiomeEnergyParticles.EdgeColor = new Color(62, 55, 110);
                    BiomeEnergyParticles.CenterColor = new Color(255, 143, 255);
                    break;
                case Attunement.Marine:
                    BiomeEnergyParticles.EdgeColor = new Color(27, 59, 101);
                    BiomeEnergyParticles.CenterColor = new Color(27, 112643, 255);
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

    public class MendedBiomeBladeVisuals : ModProjectile //Visuals
    {
        private Player Owner => Main.player[projectile.owner];
        public bool OwnerOnGround => Main.tile[(int)Owner.Center.X / 16, (int)(Owner.position.Y + (float)Owner.height - 1f) / 16 + 1].IsTileSolidGround() && Main.tile[(int)Owner.Center.X / 16 + Owner.direction, (int)(Owner.position.Y + (float)Owner.height - 1f) / 16 + 1].IsTileSolidGround();
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.modItem as TrueBiomeBlade).CanUseItem(Owner) : false;
        public bool OwnerMayChannel => OwnerCanUseItem && Owner.Calamity().mouseRight && Owner.active && !Owner.dead && Owner.StandingStill() && !Owner.mount.Active && OwnerOnGround;
        public ref float ChanneledState => ref projectile.ai[0];
        public ref float ChannelTimer => ref projectile.ai[1];
        public ref float Initialized => ref projectile.localAI[0];

        private Item associatedItem;
        const int ChannelTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mended Biome Blade");
        }
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TrueBiomeBlade";
        public bool drawIndrawHeldProjInFrontOfHeldItemAndArms = true;
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public CurveSegment anticipation = new CurveSegment(EasingType.SineOut, 0f, 1f, 0.35f);
        public CurveSegment thrust = new CurveSegment(EasingType.ExpIn, 0.85f, 1.35f, -1.45f);
        public CurveSegment bounceback = new CurveSegment(EasingType.SineOut, 0.95f, -0.1f, 0.1f);
        internal float SwordHeight() => PiecewiseAnimation(ChannelTimer / (float)ChannelTime, new CurveSegment[] { anticipation, thrust, bounceback });

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<TrueBiomeBlade>())
                {
                    projectile.Kill();
                    return;
                }

                if (Owner.whoAmI == Main.myPlayer)
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact);

                associatedItem = Owner.HeldItem;
                //Switch up the attunements
                Attunement? temporaryAttunementStorage = (associatedItem.modItem as TrueBiomeBlade).mainAttunement;
                (associatedItem.modItem as TrueBiomeBlade).mainAttunement = (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement;
                (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement = temporaryAttunementStorage;
                Initialized = 1f;
            }

            if (!OwnerMayChannel && ChanneledState == 0f) //IF the channeling gets interrupted for any reason
            {
                projectile.Center = Owner.Top + new Vector2(18f, 0f);
                ChanneledState = 1f;
                projectile.timeLeft = 60;
                return;
            }

            if (ChanneledState == 0f)
            {
                Owner.heldProj = projectile.whoAmI;

                projectile.Center = Owner.Center + new Vector2(16f * Owner.direction + Owner.direction < 0 ? -26f : 16f, -30 * SwordHeight() + 10f);
                projectile.rotation = MathHelper.PiOver4 + MathHelper.PiOver2; // No more silly turnaround with the repaired one?
                ChannelTimer++;
                projectile.timeLeft = 60;

                if (ChannelTimer >= ChannelTime)
                {
                    Attune((TrueBiomeBlade)associatedItem.modItem);
                    projectile.timeLeft = 120;
                    ChanneledState = 2f; //State where it stays invisible doing nothing. Acts as a cooldown

                    Color particleColor = (associatedItem.modItem as TrueBiomeBlade).GetAttunementInfo((associatedItem.modItem as TrueBiomeBlade).mainAttunement).color;

                    for (int i = 0; i <= 5; i++)
                    {
                        Vector2 displace = Vector2.UnitX * 20 * Main.rand.NextFloat(-1f, 1f);
                        Particle Glow = new GenericBloom(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, 0.02f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(30));
                        GeneralParticleHandler.SpawnParticle(Glow);
                    }
                    for (int i = 0; i <= 10; i++)
                    {
                        Vector2 displace = Vector2.UnitX * 16 * Main.rand.NextFloat(-1f, 1f);
                        Particle Sparkle = new GenericSparkle(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, particleColor, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }
            }

            if (ChanneledState == 1f)
                projectile.position += Vector2.UnitY * -0.3f * (1f + projectile.timeLeft / 60f);
        }

        public void Attune(TrueBiomeBlade item)
        {
            bool jungle = Owner.ZoneJungle;
            bool snow = Owner.ZoneSnow;
            bool evil = Owner.ZoneCorrupt || Owner.ZoneCrimson;
            bool desert = Owner.ZoneDesert;
            bool hell = Owner.ZoneUnderworldHeight;
            bool holy = Owner.ZoneHoly;
            bool astral = Owner.Calamity().ZoneAstral;
            bool marine = Owner.Calamity().ZoneAbyss || Owner.Calamity().ZoneSunkenSea;

            Attunement attunement = Attunement.Default;

            if (jungle)
                attunement = Attunement.Tropical;
            if (desert || hell)
                attunement = Attunement.Hot;
            if (snow)
                attunement = Attunement.Cold;
            if (evil)
                attunement = Attunement.Evil;
            if (holy)
                attunement = Attunement.Holy;
            if (astral)
                attunement = Attunement.Astral;
            if (marine)
                attunement = Attunement.Marine;

            //If the owner already had the attunement , break out of it (And unswap)
            if (item.secondaryAttunement == attunement)
            {
                Main.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                item.secondaryAttunement = item.mainAttunement;
                item.mainAttunement = attunement;
                return;
            }

            Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
            item.mainAttunement = attunement;
        }

        public override void Kill(int timeLeft)
        {
            if (associatedItem == null)
            {
                return;
            }
            //If we swapped out the main attunement for the second one despite the second attunement being empty at the time, unswap them.
            if ((associatedItem.modItem as TrueBiomeBlade).mainAttunement == null && (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement != null)
            {
                (associatedItem.modItem as TrueBiomeBlade).mainAttunement = (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement;
                (associatedItem.modItem as TrueBiomeBlade).secondaryAttunement = null;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (ChanneledState == 0f && ChannelTimer > 6f)
                return base.PreDraw(spriteBatch, lightColor);

            else if (ChanneledState == 1f)
            {
                Texture2D tex = GetTexture(Texture);
                Vector2 squishyScale = new Vector2(Math.Abs((float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * projectile.timeLeft / 30f)), 1f);
                SpriteEffects flip = (float)Math.Sin(MathHelper.Pi + MathHelper.TwoPi * projectile.timeLeft / 30f) > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(tex, projectile.position - Main.screenPosition, null, lightColor * (projectile.timeLeft / 60f), 0, tex.Size() / 2, squishyScale * (2f - (projectile.timeLeft / 60f)), flip, 0);

                return false;
            }

            else
                return false;
        }
    }
}
