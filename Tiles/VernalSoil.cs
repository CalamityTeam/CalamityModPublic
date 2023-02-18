using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class VernalSoil : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 38;
            ItemDrop = ModContent.ItemType<Items.Placeables.VernalSoil>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Vernal Soil");
            AddMapEntry(new Color(80, 120, 0), name);
            HitSound = SoundID.Dig;
        }

        public override void RandomUpdate(int i, int j)
        {
            bool underground = j >= (int)Main.worldSurface - 1 && j < Main.maxTilesY - 20;
            if (underground)
            {
                int j2 = j - 1;
                if (j2 < 10)
                    j2 = 10;

                if (Main.tile[i, j2].LiquidAmount == 0)
                {
                    if (WorldGen.genRand.NextBool(15))
                    {
                        ushort tileTypeToPlace = (ushort)ModContent.TileType<GiantPlanteraBulb>();
                        int tileTypeToPlaceThickness = 5;
                        bool placeBulb = true;
                        int minDistanceFromOtherBulbs = 10;
                        for (int k = i - minDistanceFromOtherBulbs; k < i + minDistanceFromOtherBulbs; k += 2)
                        {
                            for (int l = j - minDistanceFromOtherBulbs; l < j + minDistanceFromOtherBulbs; l += 2)
                            {
                                if (k > tileTypeToPlaceThickness && k < Main.maxTilesX - tileTypeToPlaceThickness && l > tileTypeToPlaceThickness && l < Main.maxTilesY - tileTypeToPlaceThickness && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == tileTypeToPlace)
                                {
                                    placeBulb = false;
                                    break;
                                }
                            }
                        }

                        if (placeBulb)
                        {
                            if (i < tileTypeToPlaceThickness || i > Main.maxTilesX - tileTypeToPlaceThickness || j2 < tileTypeToPlaceThickness || j2 > Main.maxTilesY - tileTypeToPlaceThickness)
                                return;

                            bool placeTile = true;
                            for (int i2 = i - 2; i2 < i + 3; i2++)
                            {
                                for (int j3 = j2 - 4; j3 < j2 + 1; j3++)
                                {
                                    if (Main.tile[i2, j3] == null)
                                        return;

                                    if (Main.tile[i2, j3].HasTile)
                                        placeTile = false;
                                }

                                if (Main.tile[i2, j2 + 1] == null)
                                    return;

                                if (!WorldGen.SolidTile2(i2, j2 + 1))
                                    placeTile = false;
                            }

                            if (placeTile)
                            {
                                WorldGen.PlaceObject(i, j2, tileTypeToPlace, true);
                                NetMessage.SendObjectPlacment(-1, i, j2, tileTypeToPlace, 0, 0, -1, -1);

                                // Spread of Chlorophyte Partisan clouds if the bulb spawns while a player is near
                                bool isPlayerNear = WorldGen.PlayerLOS(i, j2);
                                if (isPlayerNear)
                                {
                                    float projectileVelocity = 6f;
                                    int projType = ProjectileID.SporeCloud;
                                    int npcType = NPCID.Spore;
                                    Vector2 spawn = new Vector2(i * 16 + 8, j2 * 16 + 8);
                                    Vector2 dustSpawn = new Vector2(i * 16, j2 * 16);
                                    SoundEngine.PlaySound(SoundID.Item73, spawn);
                                    Vector2 destination = new Vector2(i * 16 + 8, (j2 - 2) * 16 + 8) - spawn;
                                    destination.Normalize();
                                    destination *= projectileVelocity;
                                    int numProj = 15;
                                    int numNPCs = 5;
                                    float rotation = MathHelper.ToRadians(100);

                                    for (int projIndex = 0; projIndex < numProj; projIndex++)
                                    {
                                        Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, projIndex / (float)(numProj - 1))) * (Main.rand.NextFloat() + 0.25f);

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(new EntitySource_TileUpdate(i, j2), spawn, perturbedSpeed, projType, 0, 0f, Player.FindClosest(new Vector2(i * 16, j2 * 16), 16, 16));

                                        perturbedSpeed *= 2f;
                                        Dust dust = Dust.NewDustDirect(dustSpawn, 16, 16, DustID.JungleSpore, perturbedSpeed.X, perturbedSpeed.Y, 250);
                                        dust.fadeIn = 0.7f;
                                        Dust.NewDustDirect(dustSpawn, 16, 16, (!WorldGen.genRand.NextBool(3) && Main.hardMode) ? DustID.Plantera_Pink : DustID.Plantera_Green, perturbedSpeed.X, perturbedSpeed.Y);
                                    }

                                    if (Main.hardMode)
                                    {
                                        for (int npcIndex = 0; npcIndex < numNPCs; npcIndex++)
                                        {
                                            Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, npcIndex / (float)(numNPCs - 1))) * (Main.rand.NextFloat() + 0.5f) * 0.5f;

                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                int spore = NPC.NewNPC(new EntitySource_TileUpdate(i, j2), (int)spawn.X, (int)spawn.Y, npcType, 0, -1f);
                                                Main.npc[spore].velocity.X = perturbedSpeed.X;
                                                Main.npc[spore].velocity.Y = perturbedSpeed.Y;
                                                Main.npc[spore].netUpdate = true;
                                            }

                                            perturbedSpeed *= 2f;
                                            Dust dust = Dust.NewDustDirect(dustSpawn, 16, 16, DustID.JungleSpore, perturbedSpeed.X, perturbedSpeed.Y, 250);
                                            dust.fadeIn = 0.7f;
                                            Dust.NewDustDirect(dustSpawn, 16, 16, DustID.Plantera_Pink, perturbedSpeed.X, perturbedSpeed.Y);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
