using CalamityMod.Items.Materials;
using CalamityMod.Buffs.Potions;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader.IO;
using static CalamityMod.CalamityUtils;
using static CalamityMod.Items.Weapons.Melee.FourSeasonsGalaxia;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Items.Weapons.Melee
{
    public class FourSeasonsGalaxia : ModItem
    {
        public enum Attunement : byte { SuperPogo, Whirlwind, Shockwave, FlailBlade }
        public Attunement? mainAttunement = null;

        //Used for passive effects? Unless we make em different idk lol
        public int UseTimer = 0;
        public bool OnHitProc = false;

        #region stats
        public static int WhirlwindAttunement_BaseDamage = 400;
        public static int WhirlwindAttunement_LocalIFrames = 20; //Remember its got one extra update
        public static int WhirlwindAttunement_SigilTime = 1200;
        public static float WhirlwindAttunement_BeamDamageReduction = 0.5f;
        public static float WhirlwindAttunement_BaseDamageReduction = 0.2f;
        public static float WhirlwindAttunement_FullChargeDamageBoost = 2f;

        public static int SuperPogoAttunement_BaseDamage = 500;
        public static int SuperPogoAttunement_ShredIFrames = 10;
        public static int SuperPogoAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int SuperPogoAttunement_LocalIFramesCharged = 16;
        public static float SuperPogoAttunement_SlashDamageBoost = 5f; //Keep in mind the slice always crits
        public static int SuperPogoAttunementSlashLifesteal = 6;
        public static int SuperPogoAttunement_SlashIFrames = 60;
        public static float SuperPogoAttunement_ShotDamageBoost = 2f;

        public static int ShockwaveAttunement_BaseDamage = 2500;
        public static int ShockwaveAttunement_DashHitIFrames = 60;
        public static float ShockwaveAttunement_FullChargeBoost = 1f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float ShockwaveAttunement_MonolithDamageBoost = 2f;
        public static float ShockwaveAttunement_BlastDamageReduction = 0.6f;

        public static int FlailBladeAttunement_BaseDamage = 320;
        public static int FlailBladeAttunement_LocalIFrames = 30;
        public static int FlailBladeAttunement_FlailTime = 10;
        public static int FlailBladeAttunement_Reach = 400;
        public static float FlailBladeAttunement_ChainDamageReduction = 0.5f;
        public static float FlailBladeAttunement_GhostChainDamageReduction = 0.5f;

        //Proc coefficients. aka the likelihood of any given attack to trigger a on-hit passive.
        public static float WhirlwindAttunement_WhirlwindProc = 0.24f;
        public static float WhirlwindAttunement_SwordThrowProc = 1f;
        public static float WhirlwindAttunement_SwordBeamProc = 0.05f;

        public static float SuperPogoAttunement_ShredderProc = 0.1f;
        public static float SuperPogoAttunement_WheelProc = 0.4f;
        public static float SuperPogoAttunement_DashProc = 1f;

        public static float ShockwaveAttunement_SwordProc = 1f;
        public static float ShockwaveAttunement_MonolithProc = 1f;
        public static float ShockwaveAttunement_BlastProc = 0.5f;

        public static float FlailBladeAttunement_BladeProc = 0.1f;
        public static float FlailBladeAttunement_ChainProc = 0.05f;
        public static float FlailBladeAttunement_GhostChainProc = 0.1f;
        #endregion

        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra"; //Just in case the player SOMEHOW gets to swing galaxia itself. Sprite also used as the base for other attacks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxia");
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "FUNCTION_EXTRA\n" +
                               "FUNCTION_PASSIVE\n" +
                               "Use RMB to cycle the sword's attunement forward or backwards depending on the position of your cursor\n" +
                               "Active attunement : None\n" +
                               "Passive blessing: None\n"); ;
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

                if (l.text.StartsWith("FUNCTION_PASSIVE"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = info.color;
                    l.text = info.passive_desc;
                }

                if (l.text.StartsWith("Active attunement"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor = Color.Lerp(info.color, info.color2, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);
                    l.text = "Active Attumenent : [" + info.name + "]";
                }

                if (l.text.StartsWith("Passive blessing"))
                {
                    AttunementInfo info = GetAttunementInfo(mainAttunement);
                    l.overrideColor =  info.passive_color;
                    l.text = "Passive blessing : [" + info.passive_name + "]";
                }
            }
        }

        internal struct AttunementInfo
        {
            public string name;
            public string function_description;
            public string function_extra;
            public string passive_name;
            public string passive_desc;
            public Color color;
            public Color color2;
            public Color passive_color;
        }

        internal AttunementInfo GetAttunementInfo(Attunement? attunement)
        {
            AttunementInfo AttunementInfo = new AttunementInfo();

            switch (attunement)
            {
                case Attunement.Whirlwind:
                    AttunementInfo.name = "Phoenix's Pride";
                    AttunementInfo.function_description = "Hold LMB to swing Galaxia around you, powering up as it spins. Homing cosmic bolts get released around you as you spin";
                    AttunementInfo.function_extra = "Releasing LMB during a spin will throw the sword out alongside a blast of 6 stronger cosmic bolts";
                    AttunementInfo.color = new Color(255, 87, 0);
                    AttunementInfo.color2 = new Color(255, 143, 0);
                    break;
                case Attunement.SuperPogo:
                    AttunementInfo.name = "Polaris's Gaze"; //It carries the mark of the Northern Star
                    AttunementInfo.function_description = "Channels the mark of the Northern Star into a short ranged shredding blade, surrounded by spinning stars. The blade powers up over time and when hitting enemies";
                    AttunementInfo.function_extra = "Releasing LMB sends the charged star flying. Using LMB right after it makes the player perform a dash towards the star, releasing cosmic bolts at the end of the lunge";
                    AttunementInfo.color = new Color(128, 189, 255);
                    AttunementInfo.color2 = new Color(255, 128, 140);
                    break;
                case Attunement.Shockwave:
                    AttunementInfo.name = "Andromeda's Stride"; //EHEEHEHEHE GOD ERASING BECAUSE THE ANDROMEDA BOSS WAS SCRAPPED (ALSO KNOWN AS A "GOD" BEING "ERASED") EHEHEHE
                    AttunementInfo.function_description = "Hold LMB to charge up a god-erasing sword thrust, and release to unleash the devastating blow. Small cosmic bolts are released as you charge the sword";
                    AttunementInfo.function_extra = "Striking the ground with the charge will create an impact so powerful large homing cosmic energies will rise from the ground";
                    AttunementInfo.color = new Color(132, 128, 255);
                    AttunementInfo.color2 = new Color(194, 166, 255);
                    break;
                case Attunement.FlailBlade:
                    AttunementInfo.name = "Aries's Wrath";
                    AttunementInfo.function_description = "Send out Galaxia flying, circling at your cursors' position, connected to you by constellations";
                    AttunementInfo.function_extra = "Enemy hits explode into extra homing cosmic bolts";
                    AttunementInfo.color = new Color(196, 89, 201);
                    AttunementInfo.color2 = new Color(255, 0, 0);
                    break;
                default:
                    AttunementInfo.name = "None";
                    AttunementInfo.function_description = "Does nothing... yet";
                    AttunementInfo.function_extra = "Upgrading the sword let it break free from its earthly boundaries. You now have access to every single attunement at all times.";
                    AttunementInfo.color = new Color(163, 163, 163);
                    AttunementInfo.color2 = new Color(163, 163, 163);
                    break;
            }

            switch (attunement)
            {
                case Attunement.Whirlwind:
                case Attunement.FlailBlade:
                    AttunementInfo.passive_name = "Capricorn's Blessing";
                    AttunementInfo.passive_desc = "IDK";
                    AttunementInfo.passive_color = new Color(76, 137, 237);
                    break;
                case Attunement.SuperPogo:
                case Attunement.Shockwave:
                    AttunementInfo.passive_name = "Cancer's Blessing";
                    AttunementInfo.passive_desc = "IDK";
                    AttunementInfo.passive_color = new Color(203, 25, 119);
                    break;
                default:
                    AttunementInfo.passive_name = "None";
                    AttunementInfo.passive_desc = "Based on your current attunement, you are granted a passive blessing";
                    AttunementInfo.passive_color = new Color(163, 163, 163);
                    break;
            }
            return AttunementInfo;
        }

        #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 108;
            item.damage = 99;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 24f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<OmegaBiomeBlade>());
            recipe.AddIngredient(ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ItemType<DarksunFragment>(), 8);
            recipe.AddTile(TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<FourSeasonsGalaxia>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);
            (clone as FourSeasonsGalaxia).mainAttunement = (item.modItem as FourSeasonsGalaxia).mainAttunement;

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();
            (clone as FourSeasonsGalaxia).mainAttunement = mainAttunement;

            return clone;
        }

        public override TagCompound Save()
        {
            int attunement1 = mainAttunement == null ? 1 : (int)mainAttunement;
            TagCompound tag = new TagCompound
            {
                { "mainAttunement", attunement1 }
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");
            if (attunement1 == -1)
                mainAttunement = Attunement.Whirlwind;
            else
                mainAttunement = (Attunement?)attunement1;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)mainAttunement);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = (Attunement?)reader.ReadByte();
        }

#endregion

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (mainAttunement == null)
                return false;
            return true;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;

            if (CanUseItem(player))
            {
                player.Calamity().LungingDown = false;
            }
            else
            {
                UseTimer++;
            }

            //Change the swords function based on its attunement
            switch (mainAttunement)
            {
                case Attunement.Whirlwind:
                    item.damage = WhirlwindAttunement_BaseDamage;
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<PhoenixsPride>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;
                    break;
                case Attunement.SuperPogo:
                    item.damage = SuperPogoAttunement_BaseDamage;
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<PolarissGaze>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;
                    break;
                case Attunement.Shockwave:
                    item.damage = ShockwaveAttunement_BaseDamage;
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<AndromedasStride>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;
                    break;
                case Attunement.FlailBlade:
                    item.damage = FlailBladeAttunement_BaseDamage;
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<AriessWrath>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;
                    break;
                default:
                    mainAttunement = Attunement.Whirlwind;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileID.PurificationPowder;
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;
                    break;
            }


            if (player.whoAmI != Main.myPlayer)
                return;

            //PAssive effetcsts : break
            switch (mainAttunement)
            {
                case Attunement.SuperPogo:
                case Attunement.Shockwave:
                    break;
                case Attunement.FlailBlade:
                case Attunement.Whirlwind:
                    break;
                default:
                    break;
            }


            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<GalaxiaVisuals>() && n.owner == player.whoAmI))
                    return;

                Projectile.NewProjectile(player.Top, Vector2.Zero, ProjectileType<GalaxiaVisuals>(), 0, 0, player.whoAmI, 0, Math.Sign(player.position.X - Main.MouseWorld.X));
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<PhoenixsPride>() ||
             n.type == ProjectileType<AndromedasStride>() ||
             n.type == ProjectileType<PolarissGaze>() ||
             n.type == ProjectileType<AriessWrath>()
            ));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (mainAttunement == null)
                mainAttunement = Attunement.Whirlwind;

            Texture2D itemTexture = GetTexture((mainAttunement == Attunement.SuperPogo || mainAttunement == Attunement.Shockwave) ? "CalamityMod/Items/Weapons/Melee/GalaxiaRed" : "CalamityMod/Items/Weapons/Melee/GalaxiaBlue");
            spriteBatch.Draw(itemTexture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D itemTexture = GetTexture((mainAttunement == Attunement.SuperPogo || mainAttunement == Attunement.Shockwave) ? "CalamityMod/Items/Weapons/Melee/GalaxiaRed" : "CalamityMod/Items/Weapons/Melee/GalaxiaBlue");
            spriteBatch.Draw(itemTexture, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class GalaxiaVisuals : ModProjectile //Visuals. Now much simpler than before!
    {
        private Player Owner => Main.player[projectile.owner];
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.modItem as FourSeasonsGalaxia).CanUseItem(Owner) : false;
        public ref float Initialized => ref projectile.ai[0];
        public ref float CycleDirection => ref projectile.ai[1];

        private Item associatedItem;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Formation");
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 2;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 40;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<FourSeasonsGalaxia>())
                {
                    projectile.Kill();
                    return;
                }

                projectile.Center = Owner.Center;
                associatedItem = Owner.HeldItem;

                //Switch up the attunements
                Reattune((FourSeasonsGalaxia)associatedItem.modItem);

                //Do particles around the player 

                Color particleColor = (associatedItem.modItem as FourSeasonsGalaxia).GetAttunementInfo((associatedItem.modItem as FourSeasonsGalaxia).mainAttunement).color;
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

                Initialized = 1f;
            }
        }

        public void Reattune(FourSeasonsGalaxia item)
        {
            Attunement attunement;
            Vector2[] StarPositions;
            Vector2[] ExtraLines; //Extra lines vector give us an indication of the index between the 2 stars we need to connect
            List<int> IgnoredLines = new List<int>();

            Particle Star;
            Particle Line;
            Color StarColor;

            if (CycleDirection == -1) //Cycles goes Whirlwind => Flailblade => Superpogo => Shockwave
            {
                switch (item.mainAttunement)
                {
                    case Attunement.Whirlwind: //Switching to the flailblade attunement. 
                        attunement = Attunement.FlailBlade;
                        break;
                    case Attunement.FlailBlade: //Switching to the superpogo attunement. 
                        attunement = Attunement.SuperPogo;
                        break;
                    case Attunement.SuperPogo: //Switching to the shockwave attunement. 
                        attunement = Attunement.Shockwave;
                        break;
                    case Attunement.Shockwave: //Switching to the whirlwind attunement. 
                    default:
                        attunement = Attunement.Whirlwind;
                        break;
                }
            }
            else //Cycles goes Whirlwind <= Flailblade <= Superpogo <= Shockwave
            {
                switch (item.mainAttunement)
                {
                    case Attunement.Whirlwind: //Switching to the shockwave attunement. 
                        attunement = Attunement.Shockwave;
                        break;
                    case Attunement.Shockwave: //Switching to the superpogo attunement. 
                        attunement = Attunement.SuperPogo;
                        break;
                    case Attunement.SuperPogo: //Switching to the flailblade attunement. 
                        attunement = Attunement.FlailBlade;
                        break;
                    case Attunement.FlailBlade: //Switching to the whirlwind attunement. 
                    default:
                        attunement = Attunement.Whirlwind;
                        break;
                }
            }

            switch (attunement)
            {
                case Attunement.FlailBlade: // Drawing Aries
                    StarPositions = new Vector2[] { new Vector2(-160, -150), new Vector2(45, -170), new Vector2(137, 40), new Vector2(146, 126), new Vector2(129, 151) };
                    ExtraLines = new Vector2[] { };
                    StarColor = Color.Orchid;
                    break;
                case Attunement.SuperPogo: //Drawing Ursa Minor
                    StarPositions = new Vector2[] { new Vector2(69, -188), new Vector2(18, -122), new Vector2(-23, -39), new Vector2(-13, 63), new Vector2(42, 147), new Vector2(-8, 184), new Vector2(-61, 83) };
                    ExtraLines = new Vector2[] { new Vector2(3, 6) };
                    StarColor = Color.CornflowerBlue;
                    break;
                case Attunement.Shockwave: //Drawing Andromeda
                    //https://media.discordapp.net/attachments/802291445360623686/934200685254291546/unknown.png
                    StarPositions = new Vector2[] { 
                        new Vector2(-210, -46), new Vector2(-150, -35), new Vector2(-69, 18), new Vector2(33, 61), new Vector2(127, 72), //The horizontal line
                        new Vector2(-41, -27), new Vector2(-41, -67), new Vector2(-100, -124), new Vector2(-160, -130), //The first branch from the left
                        new Vector2(15, 147), new Vector2(35, 126), new Vector2(37, 23), new Vector2(67, -47), new Vector2(126, -109), new Vector2(146, -136), //The second branch from the left
                        new Vector2(95, -117)
                    };
                    ExtraLines = new Vector2[] { new Vector2(2, 5) , new Vector2(13, 15) };
                    IgnoredLines.Add(5);
                    IgnoredLines.Add(9);
                    IgnoredLines.Add(15);
                    StarColor = Color.MediumSlateBlue;
                    break;
                case Attunement.Whirlwind: //Drawing Phoenix
                default: 
                    StarPositions = new Vector2[] { new Vector2(-206, -99), new Vector2(-150, -43), new Vector2(-120, -146), new Vector2(-60, -71), new Vector2(-106, 71), new Vector2(-59, 138), new Vector2(116, -22),//The main line
                    new Vector2(246, -26),  new Vector2(192, 36), new Vector2(138, 81), //The side bit
                    new Vector2(88, -107), new Vector2(84, -75) }; //Complete the loop
                    ExtraLines = new Vector2[] { new Vector2(3, 10), new Vector2(11, 6) };
                    IgnoredLines.Add(10);
                    StarColor = Color.OrangeRed;
                    break;
            }

            //Only draw the cool effect if you have enough particle slots left. Itd look really ugly to have a half complete constellation
            if (GeneralParticleHandler.FreeSpacesAvailable() > StarPositions.Length * 2)
            {
                for (int i = 0; i < StarPositions.Length; i++)
                {
                    //The polar star gets bigger than the others. Also since andromeda has so many stars, make em smaller
                    Star = new GenericSparkle(Owner.Center + StarPositions[i], Vector2.Zero, Color.White, StarColor, (attunement == Attunement.SuperPogo && i == 0) ? 3f : Main.rand.NextFloat(1f, 1.5f) * (attunement == Attunement.Shockwave ? 0.8f : 1f), 20, 0f, 3f);
                    GeneralParticleHandler.SpawnParticle(Star);

                    if (i > 0 && !IgnoredLines.Contains(i))
                    {
                        Line = new BloomLineVFX(Owner.Center + StarPositions[i - 1], StarPositions[i] - StarPositions[i - 1], 0.5f, StarColor, 20, true);
                        GeneralParticleHandler.SpawnParticle(Line);
                    }
                }

                for (int i = 0; i < ExtraLines.Length; i++)
                {
                    Line = new BloomLineVFX(Owner.Center + StarPositions[(int)ExtraLines[i].Y], StarPositions[(int)ExtraLines[i].X] - StarPositions[(int)ExtraLines[i].Y], 0.5f, StarColor, 20, true);
                    GeneralParticleHandler.SpawnParticle(Line);
                }
            }

            //Chunger
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThunderStrike"), projectile.Center);
            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;
            item.mainAttunement = attunement;
        } 
    }
}
