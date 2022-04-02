using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
	public class Skyfin : ModNPC
    {
        public ref float AttackState => ref npc.ai[0];
        public ref float AttackTimer => ref npc.ai[1];
        public Player Target => Main.player[npc.target];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skyfin");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 22;
            npc.aiStyle = aiType = -1;

            npc.damage = 12;
            npc.lifeMax = 70;
            npc.defense = 6;
            npc.knockBackResist = 1f;

            if (CalamityWorld.downedPolterghast)
            {
                npc.knockBackResist = 0.8f;
                npc.damage = 88;
                npc.lifeMax = 3025;
                npc.defense = 18;
                npc.DR_NERD(0.05f);
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 38;
                npc.lifeMax = 220;
                npc.DR_NERD(0.05f);
            }

            npc.value = Item.buyPrice(0, 0, 3, 65);
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SkyfinBanner>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
		}

        public override void AI()
        {
            npc.TargetClosest(false);
            int idealDirection = (npc.velocity.X > 0).ToDirectionInt();
            npc.spriteDirection = idealDirection;

            switch ((int)AttackState)
            {
                // Rise upward.
                case 0:
                    Vector2 flyDestination = Target.Center + new Vector2((Target.Center.X < npc.Center.X).ToDirectionInt() * 400f, -240f);
                    Vector2 idealVelocity = npc.SafeDirectionTo(flyDestination) * 12f;
                    npc.velocity = (npc.velocity * 29f + idealVelocity) / 29f;
                    npc.velocity = npc.velocity.MoveTowards(idealVelocity, 1.5f);

                    // Decide rotation.
                    npc.rotation = npc.velocity.ToRotation() + (npc.spriteDirection > 0).ToInt() * MathHelper.Pi;

                    if (npc.WithinRange(flyDestination, 40f) || AttackTimer > 150f)
                    {
                        AttackState = 1f;
                        npc.velocity *= 0.65f;
                        npc.netUpdate = true;
                    }
                    break;

                // Slow down and look at the target.
                case 1:
                    npc.spriteDirection = (Target.Center.X > npc.Center.X).ToDirectionInt();
                    npc.velocity *= 0.97f;
                    npc.velocity = npc.velocity.MoveTowards(Vector2.Zero, 0.25f);
                    npc.rotation = npc.rotation.AngleTowards(npc.AngleTo(Target.Center) + (npc.spriteDirection > 0).ToInt() * MathHelper.Pi, 0.2f);

                    // Charge once sufficiently slowed down.
                    float chargeSpeed = 11.5f;
                    if (CalamityWorld.downedAquaticScourge)
                        chargeSpeed += 4f;
                    if (CalamityWorld.downedPolterghast)
                        chargeSpeed += 3.5f;
                    if (npc.velocity.Length() < 1.25f)
                    {
                        Main.PlaySound(SoundID.DD2_WyvernDiveDown, npc.Center);
                        for (int i = 0; i < 36; i++)
                        {
                            Dust acid = Dust.NewDustPerfect(npc.Center, (int)CalamityDusts.SulfurousSeaAcid);
                            acid.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 6f;
                            acid.scale = 1.1f;
                            acid.noGravity = true;
                        }

                        AttackState = 2f;
                        AttackTimer = 0f;
                        npc.velocity = npc.SafeDirectionTo(Target.Center) * chargeSpeed;
                        npc.netUpdate = true;
                    }
                    break;

                // Charge and swoop.
                case 2:
                    float angularTurnSpeed = MathHelper.Pi / 300f;
                    idealVelocity = npc.SafeDirectionTo(Target.Center);
                    Vector2 leftVelocity = npc.velocity.RotatedBy(-angularTurnSpeed);
                    Vector2 rightVelocity = npc.velocity.RotatedBy(angularTurnSpeed);
                    if (leftVelocity.AngleBetween(idealVelocity) < rightVelocity.AngleBetween(idealVelocity))
                        npc.velocity = leftVelocity;
                    else
                        npc.velocity = rightVelocity;

                    // Decide rotation.
                    npc.rotation = npc.velocity.ToRotation() + (npc.spriteDirection > 0).ToInt() * MathHelper.Pi;

                    if (AttackTimer > 50f)
                    {
                        AttackState = 0f;
                        AttackTimer = 0f;
                        npc.velocity = Vector2.Lerp(npc.velocity, -Vector2.UnitY * 8f, 0.14f);
                        npc.netUpdate = true;
                    }
                    break;
            }
            AttackTimer++;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 5)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                    npc.frame.Y = 0;
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulfuricScale>(), 2, 1, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<SkyfinBombers>(), CalamityWorld.downedAquaticScourge, 0.05f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/SkyfinGore3"), npc.scale);
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}
