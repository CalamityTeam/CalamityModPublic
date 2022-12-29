
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World.Planets
{
    public class MudPlanet : Planetoid
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int radius = _random.Next(12, 19);

            if (!CheckIfPlaceable(origin, radius, structures))
            {
                return false;
            }

            PlacePlanet(origin, radius);

            return base.Place(origin, structures);
        }

        public void PlacePlanet(Point origin, int radius)
        {
            ShapeData shape = new ShapeData();

            //Create shapes for each layer
            GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
            WorldUtils.Gen(origin, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
            {
                blotchMod.Output(shape)
            }));

            //Place layers
            WorldUtils.Gen(origin, new ModShapes.All(shape), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceTile(TileID.Mud),
                new Actions.PlaceWall(WallID.MudUnsafe)
            }));

            //Place stone
            int numStone = _random.Next(3, 9);
            for (int i = 0; i < numStone; i++)
            {
                int x = _random.Next(origin.X - radius + 3, origin.X + radius - 2);
                int y = _random.Next(origin.Y - radius + 3, origin.Y + radius - 2);
                WorldGen.TileRunner(x, y, _random.NextFloat(5f, 9f), _random.Next(5, 15), Main.getGoodWorld ? TileID.WoodenSpikes : TileID.Stone);
            }

            //Place grass, remove wall and occasionally smooth
            WorldUtils.Gen(origin, new ModShapes.InnerOutline(shape), Actions.Chain(new GenAction[]
            {
                new Actions.ClearWall(true),
                new Modifiers.OnlyTiles(new ushort[] { TileID.Mud }),
                new Modifiers.IsTouchingAir(true),
                new Actions.SetTile(TileID.JungleGrass, false, true),
                new Modifiers.Conditions(new CustomConditions.RandomChance(3)),
                new Actions.Smooth(),
                new Actions.SetFrames(true)
            }));

            if (_random.NextBool())
            {
                //Make a beehive in the planet.
                //To do this, we need to create a shape, outline it with hive blocks (we'll need to do this twice to create a nice shape)
                ShapeData hive = new ShapeData();
                int blobs = _random.Next(3, 6);
                float smallerRadius = radius * 0.55f;
                int minSize = (int)(radius * 0.4f);
                int maxSize = (int)(radius * 0.6f);
                for (int k = 0; k < blobs; k++)
                {
                    int moveX = _random.Next(-(int)smallerRadius, (int)smallerRadius);
                    int moveY = _random.Next(-(int)smallerRadius, (int)smallerRadius);
                    WorldUtils.Gen(origin,
                        new Shapes.Circle(_random.Next(minSize, maxSize), _random.Next(minSize, maxSize)),
                        Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Offset(moveX, moveY),
                            new Modifiers.Blotches(2, 0.12),
                            new Actions.ClearTile(true).Output(hive),
                            new Actions.PlaceWall(WallID.HiveUnsafe)
                        })
                    );
                }
                ShapeData hiveOutline = new ShapeData();
                WorldUtils.Gen(origin, new ModShapes.InnerOutline(hive, true), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceTile(TileID.Hive),
                    new Actions.ClearWall().Output(hiveOutline),
                    new Modifiers.Conditions(new CustomConditions.IsNotTouchingAir(false)),
                    new Actions.PlaceWall(WallID.MudUnsafe)
                }));
                hive.Subtract(hiveOutline, origin, origin);
                WorldUtils.Gen(origin, new ModShapes.InnerOutline(hive, true), new Actions.PlaceTile(TileID.Hive).Output(hiveOutline));
                hive.Subtract(hiveOutline, origin, origin);
                WorldUtils.Gen(origin, new ModShapes.All(hive), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Conditions(new CustomConditions.RandomChance(7)),
                    new Actions.SetLiquid(2, 255)
                }));
                bool placedChest = false;
                while (!placedChest)
                {
                    int testX = origin.X + _random.Next(-radius, radius);
                    int testY = origin.Y + _random.Next(-radius, radius);
                    if (WorldGen.EmptyTileCheck(testX - 1, testX + 1, testY - 1, testY + 1) && _tiles[testX, testY].WallType  == WallID.HiveUnsafe)
                    {
                        for (int floorX = testX - 1; floorX <= testX + 1; floorX++)
                        {
                            _tiles[floorX, testY + 2].Get<TileWallWireStateData>().HasTile = true;
                            _tiles[floorX, testY + 2].TileType = TileID.Hive;
                            WorldGen.SquareTileFrame(floorX, testY + 2);
                        }
                        //Place chest
                        int chestID = WorldGen.PlaceChest(testX, testY + 1, 21, false, 29);
                        FillHoneyChest(chestID);
                        placedChest = true;
                    }
                }
            }
            else if (_random.Next(4) <= 2)
            {
                //Create a small "cavern" inside of the planet. Filled with water and has a water chest (CUSTOM LOOT?)
                ShapeData cavernData = new ShapeData();
                float waterChance = _random.NextFloat(2.2f, 3.5f);
                WorldUtils.Gen(origin, new Shapes.Circle((int)(radius * _random.NextFloat(0.35f, 0.5f))), Actions.Chain(new GenAction[]
                {
                    new Modifiers.Blotches(),
                    new Actions.ClearTile().Output(cavernData),
                    new Modifiers.Conditions(new CustomConditions.RandomChance(waterChance)),
                    new Actions.SetLiquid(0, 255)
                }));
                shape.Subtract(cavernData, origin, origin);
                int chestY = origin.Y;
                while (!_tiles[origin.X, chestY].HasTile)
                {
                    chestY++;
                }
                for (int x = origin.X; x <= origin.X + 1; x++)
                {
                    for (int y = chestY - 2; y <= chestY - 1; y++)
                    {
                        WorldGen.KillTile(x, y);
                    }
                }
                //PLACE TILES BENEATH CHEST, IN CASE
                _tiles[origin.X, chestY].Get<TileWallWireStateData>().HasTile = true;
                _tiles[origin.X, chestY].TileType = TileID.Mud;
                _tiles[origin.X + 1, chestY].Get<TileWallWireStateData>().HasTile = true;
                _tiles[origin.X + 1, chestY].TileType = TileID.Mud;
                //Place chest
                int chestID = WorldGen.PlaceChest(origin.X, chestY - 1, 21, false, 17);
                FillChest(chestID);
            }
            else
            {
                //Glowing mushroom planetoid
                WorldUtils.Gen(origin, new ModShapes.All(shape), Actions.Chain(new GenAction[]
                {
                    new Modifiers.OnlyTiles(new ushort[] { TileID.JungleGrass }),
                    new Actions.SetTile(TileID.MushroomGrass, true, true)
                }));
            }

            //Place breakable grass and trees
            WorldUtils.Gen(origin, new ModShapes.All(shape), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.JungleGrass, TileID.MushroomGrass }),
                new Modifiers.Offset(0, -1),
                new CustomActions.JungleGrass(true),
                new Modifiers.Conditions(new CustomConditions.RandomChance(3.5f)),
                new Modifiers.Offset(0, 1),
                new CustomActions.PlaceTree()
            }));

            //Place vines
            WorldUtils.Gen(origin, new ModShapes.All(shape), Actions.Chain(new GenAction[]
            {
                new Modifiers.OnlyTiles(new ushort[] { TileID.JungleGrass }),
                new Modifiers.Conditions(new CustomConditions.RandomChance(2)),
                new Modifiers.Offset(0, 1),
                new ActionVines(3, 7, TileID.JungleVines)
            }));
        }

        //---------------------
        //WATER CHEST STUFF
        //---------------------

        private int[] FocusLoot = new int[]
        {
            ItemID.Flipper,
            ItemID.DivingHelmet,
            ItemID.Trident
        };

        private int[] PotionLoot = new int[]
        {
            ItemID.FlipperPotion,
            ItemID.RegenerationPotion,
            ItemID.WaterWalkingPotion,
            ItemID.NightOwlPotion,
            ItemID.GravitationPotion,
            ItemID.EndurancePotion
        };

        private int[] BarLoot = new int[]
        {
            WorldGen.silverBar == TileID.Silver ? ItemID.SilverBar : ItemID.TungstenBar,
            WorldGen.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar
        };

        private void FillChest(int id)
        {
            Chest chest = Main.chest[id];
            int index = 0;

            //Focus loot
            chest.item[index++].SetDefaults(_random.Next(FocusLoot));

            //Bars
            if (_random.Next(3) <= 1)
            {
                chest.item[index].SetDefaults(_random.Next(BarLoot));
                chest.item[index].SetDefaults(_random.Next(7, 15));
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.GoldCoin);
                chest.item[index++].stack = _random.Next(2, 4); //2 or 3 gold coins
            }

            //Potion loot
            if (_random.Next(2) == 0)
            {
                chest.item[index].SetDefaults(_random.Next(PotionLoot));
                chest.item[index++].stack = _random.Next(1, 4);
            }
            else //Healing potion
            {
                chest.item[index].SetDefaults(ItemID.HealingPotion);
                chest.item[index++].stack = _random.Next(3, 7);
            }

            //Weaponry
            if (_random.Next(2) == 0)
            {
                chest.item[index].SetDefaults(_random.Next(new int[] { ItemID.Shuriken, ItemID.ThrowingKnife }));
                chest.item[index++].stack = _random.Next(50, 100);
            }
            else
            {
                chest.item[index].SetDefaults(_random.Next(new int[] { ItemID.Grenade, ItemID.Bomb }));
                chest.item[index++].stack = _random.Next(5, 10);
            }

            //Recall potion
            if (_random.Next(2) == 0)
            {
                chest.item[index].SetDefaults(ItemID.RecallPotion);
                chest.item[index++].stack = _random.Next(1, 4);
            }
            else //glowsticks
            {
                chest.item[index].SetDefaults(ItemID.Glowstick);
                chest.item[index++].stack = _random.Next(18, 36);
            }

        }

        //---------------------
        //HONEY CHEST STUFF
        //---------------------

        private int[] FocusLootHoney = new int[]
        {
            ItemID.NaturesGift,
            ItemID.Bezoar,
            ItemID.SharpeningStation
        };

        private int[] PotionLootHoney = new int[]
        {
            ItemID.LifeforcePotion,
            ItemID.RegenerationPotion,
            ItemID.ManaRegenerationPotion,
            ItemID.HeartreachPotion,
            ModContent.ItemType<PhotosynthesisPotion>()
        };

        private int[] BarLootHoney = new int[]
        {
            WorldGen.silverBar == TileID.Silver ? ItemID.SilverBar : ItemID.TungstenBar,
            WorldGen.goldBar == TileID.Gold ? ItemID.GoldBar : ItemID.PlatinumBar
        };

        private void FillHoneyChest(int id)
        {
            Chest chest = Main.chest[id];
            int index = 0;

            //Focus loot
            chest.item[index++].SetDefaults(_random.Next(FocusLootHoney));

            //Bars
            if (_random.Next(3) <= 1)
            {
                chest.item[index].SetDefaults(_random.Next(BarLootHoney));
                chest.item[index].SetDefaults(_random.Next(7, 15));
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.GoldCoin);
                chest.item[index++].stack = _random.Next(3, 5); // 3 or 4 gold coins
            }

            //Potion loot
            if (_random.Next(2) == 0)
            {
                chest.item[index].SetDefaults(_random.Next(PotionLootHoney));
                chest.item[index++].stack = _random.Next(1, 4);
            }
            else //Healing potion
            {
                chest.item[index].SetDefaults(ItemID.BottledHoney);
                chest.item[index++].stack = _random.Next(3, 7);
            }

            //Weaponry
            if (_random.Next(2) == 0)
            {
                chest.item[index].SetDefaults(ItemID.Stinger);
                chest.item[index++].stack = _random.Next(4, 6);
            }
            else
            {
                chest.item[index].SetDefaults(ItemID.JungleSpores);
                chest.item[index++].stack = _random.Next(3, 5);
            }

            //Recall potion
            if (_random.Next(2) == 0)
            {
                chest.item[index].SetDefaults(ItemID.RecallPotion);
                chest.item[index++].stack = _random.Next(1, 4);
            }
            else //glowsticks
            {
                chest.item[index].SetDefaults(ItemID.YellowTorch);
                chest.item[index++].stack = _random.Next(18, 36);
            }

        }
    }
}
