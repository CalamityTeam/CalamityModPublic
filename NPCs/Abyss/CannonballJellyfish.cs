using System;
using CalamityMod.BiomeManagers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    public class CannonballJellyfish : ModNPC
    {
        public bool hasBeenHit = false;
        public bool dying = false;
        public bool shouldTarget = false;
        public int boomTimer = -1;
        public int currentFrame = 0;
        
        public const int dyingDuration = 75;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 50; //the damage is amplified on explosion, contact can be lower
            NPC.width = 54;
            NPC.height = 76;
            NPC.defense = 0;
            NPC.lifeMax = 400;
            NPC.knockBackResist = 0f;
            NPC.alpha = 100;
            NPC.value = Item.buyPrice(0, 0, 1, 5);
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CannonballJellyfishBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbyssLayer1Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.CannonballJellyfish")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(shouldTarget);
            writer.Write(boomTimer);
            writer.Write(currentFrame);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            shouldTarget = reader.ReadBoolean();
            boomTimer = reader.ReadInt32();
            currentFrame = reader.ReadInt32();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => !dying && base.CanHitPlayer(target, ref cooldownSlot);

        private void DyingAI() 
        {
            if (boomTimer == 0)
            {
                if (!shouldTarget)
                {
                    NPC.alpha = 50;
                    shouldTarget = true;
                    boomTimer = dyingDuration;
                    NPC.noTileCollide = true;
                    NPC.netUpdate = true;
                }
                else
                {
                    Explode();
                    return;
                }
                    
            }

            if (boomTimer == 60 && !shouldTarget) //happens only once
            {
                NPC.velocity.Y = 0.75f; //sink slowly
            }
                

            if (shouldTarget) 
            {
                // Get a target
                if (NPC.target < 0 || !NPC.target.WithinBounds(Main.maxPlayers) || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.TargetClosest();
                
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                    NPC.TargetClosest();
                
                Player player = Main.player[NPC.target];
                
                if (boomTimer >= (dyingDuration - 20)) //home for the first 20 frames, not beyond that 
                {
                    float targetXDirection = player.position.X + (float)(player.width / 2);
                    float targetYDirection = player.position.Y + (float)(player.height / 2);
                    float homingSpeed = 30f;
                    Vector2 npcPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float targetXDist = targetXDirection - npcPosition.X;
                    float targetYDist = targetYDirection - npcPosition.Y;
                    float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                    if (targetDistance < 100f)
                    {
                        homingSpeed = 10f; //5
                    }
                    targetDistance = homingSpeed / targetDistance;
                    targetXDist *= targetDistance;
                    targetYDist *= targetDistance;
                    NPC.velocity.X = (NPC.velocity.X * 5f + targetXDist) / 6f;
                    NPC.velocity.Y = (NPC.velocity.Y * 5f + targetYDist) / 6f;

                    NPC.velocity *= 0.9f; //slow it down a bit to be fair
                
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

                }
                

                //apparently NPC.wet can return true even when out of water :skull:
                if (!Collision.DrownCollision(NPC.position, NPC.width, NPC.height) || Collision.SolidCollision(NPC.Center, NPC.width, NPC.height) || NPC.Hitbox.Intersects(player.Hitbox)) //explode when in contact with a tile or target
                    Explode();
            }
            else
            {
                Lighting.AddLight(NPC.Center, (175 - NPC.alpha) * 0.2f, 0, 0);
            }

            boomTimer--;
        }
        
        public override void AI()
        {
            NPC.chaseable = !dying && hasBeenHit;
            // Do the death animation once killed.
            if (dying)
            {
                DyingAI();
                return;
            }
            // Trigger the death animation
            if (NPC.life <= 1)
            {
                dying = true;
                boomTimer = boomTimer == -1 ? 60 : boomTimer;
                NPC.life = 1;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                return;
            }
            Lighting.AddLight(NPC.Center, (67 - NPC.alpha) * 1f / 220f, (218 - NPC.alpha) * 1f / 220f, (166 - NPC.alpha) * 1f / 220f);
            if (NPC.justHit)
            {
                hasBeenHit = true;
                NPC.chaseable = true;
                NPC.netUpdate = true;
            }
            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.velocity.Y = -6f;
                NPC.netUpdate = true;
            }
            if (NPC.wet)
            {
                NPC.noGravity = true;
                if (NPC.localAI[2] > 0f)
                {
                    NPC.localAI[2] -= 1f;
                }
                if (NPC.localAI[2] <= 0f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.localAI[1] += 1f;
                    }
                    else
                    {
                        NPC.localAI[1] = 0f;
                    }
                }
                NPC.velocity.Y += 0.1f;
                if (currentFrame == 5 && NPC.frameCounter == 0.0)
                    NPC.velocity.Y = -2.748f; // manually picked value :desolate:
            }
            else
            {
                NPC.noGravity = false;
                NPC.velocity.Y = 2f;
                NPC.localAI[2] = 75f;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight) //8 frames, 78 height
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter >= 7)
            {
                currentFrame = currentFrame == 7 ? 0 : currentFrame + 1;
                NPC.netUpdate = true; //update current frame variable
                NPC.frameCounter = 0;
            }
                
            NPC.frame.Y = currentFrame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer1 && spawnInfo.Water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.1f;
            }
            return 0f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            var texture = TextureAssets.Npc[NPC.type];
            Vector2 halfSizeTexture = new Vector2((float)(texture.Value.Width / 2), (float)(texture.Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Vector2 vector = center - screenPos;
            var glowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/CannonballJellyfishGlow");
            vector -= new Vector2((float)glowTexture.Value.Width, (float)(glowTexture.Value.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
            vector += halfSizeTexture * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(new Color(67, 218, 166));
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Abyss/CannonballJellyfishGlow").Value, vector,
                new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, halfSizeTexture, 1f, spriteEffects, 0f);
        }

        private void Explode()
        {
            NPC.position = NPC.Center;
            NPC.height *= 3;
            NPC.width = NPC.height;
            NPC.position -= NPC.Size * 0.5f; //hitbox expansion + adjustments
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center); //bomb sound
            foreach (Player player in Main.player.Where(player => player.active && !player.dead && NPC.Hitbox.Intersects(player.Hitbox)))
                player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage * 3, NPC.direction); //Due to how slow the jelly is, this might catch you unaware, but then you'll never get hit by it again
            for (int k = 0; k < 25; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MoonBoulder, 0f, -1f, 0, default, 2f);
                if (Main.dust.IndexInRange(dust))
                    Main.dust[dust].noGravity = true;
            }
            NPC.active = false;
            NPC.NPCLoot();
            NPC.netUpdate = true;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => target.AddBuff(BuffID.Poisoned, 60 * (this.shouldTarget ? 10 : 5)); //10 sec for explosion, 5 sec for non explosion

        public override bool CheckDead()
        {
            NPC.life = 1;
            NPC.alpha = boomTimer == -1 ? 125 : NPC.alpha;
            boomTimer = boomTimer == -1 ? 60 : boomTimer;
            dying = true;
            NPC.active = true;
            NPC.dontTakeDamage = true;
            NPC.netUpdate = true;
            return false;
        }

        public override bool? CanBeHitByItem(Player player, Item item) => dying ? false : base.CanBeHitByItem(player, item);

        public override bool? CanBeHitByProjectile(Projectile projectile) => dying ? false : base.CanBeHitByProjectile(projectile);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MoonBoulder, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.Bomb, 1, 1, 1);
            npcLoot.Add(ItemID.JellyfishNecklace, 100);
        }
    }
}
