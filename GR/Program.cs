using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace GR
{
    public class Program
    {
        public IList<Item> Items;

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome");

            var app = new Program()
            {
                Items = new List<Item>
                {
                    new Item {Name = "+5 Dexterity Vest", SellIn = 2, Quality = 50},
                    new Item {Name = "Aged Brie", SellIn = 3, Quality = 50},
                    new Item {Name = "Elixir of the Mongoose", SellIn = -1, Quality = 10},
                    new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                    new Item
                    {
                        Name = "Backstage passes to a TAFKAL80ETC concert",
                        SellIn = 10,
                        Quality = 0
                    },
                    new Item {Name = "Conjured Mana Cake", SellIn = 10, Quality = 10}
                }
            };

            app.UpdateInventory();

            var filename = $"inventory_{DateTime.Now:yyyyddMM-HHmmss}.txt";
            var inventoryOutput = JsonConvert.SerializeObject(app.Items, Formatting.Indented);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename), inventoryOutput);

            Console.ReadKey();
        }

        public void UpdateInventory()
        {
            Console.WriteLine("Updating inventory");
            foreach (var item in Items)
            {
                Console.WriteLine(" - Item: {0}", item.Name);
                if (item.Name != "Aged Brie" && !item.Name.Contains("Backstage passes"))
                {
                    if (item.Quality > 0)
                    {
                        if (item.Name != "Sulfuras, Hand of Ragnaros")//"Sulfuras", being a legendary item, never has to be sold or decreases in Quality
                        {
                            if (item.SellIn > 0)
                            {
                                if (item.Name == "Conjured Mana Cake")
                                    item.Quality = item.Quality - 2;//"Conjured" items degrade in Quality twice as fast as normal items
                                else
                                    item.Quality = item.Quality - 1;//"normal" items degrade in Quality once if sell date not passed
                            }
                        }

                    }
                }
                else
                {
                    if (item.Quality < 50) //The Quality of an item is never more than 50
                    {
                      
                        if (item.Name.Contains("Backstage passes"))
                        {
                            //Quality increases by 2 when there are 10 days or less
                            if (item.SellIn < 11)
                            {
                                if (item.Quality < 50)//The Quality of an item is never more than 50
                                {
                                    item.Quality = item.Quality + 2;
                                }
                            }
                            //Quality increases by 3 when there are 5 days or less
                            if (item.SellIn < 6)
                            {
                                if (item.Quality < 50)//The Quality of an item is never more than 50
                                {
                                    item.Quality = item.Quality + 3;
                                }
                            }
                            if (item.SellIn <= 0)
                            {
                                //Concert over Backstage passes quality should be 0
                                item.Quality = 0;

                            }
                        }
                        else
                            //"Aged Brie" actually increases in Quality the older it gets
                            item.Quality = item.Quality + 1;
                    }

                }
                //"Sulfuras", being a legendary item, never has to be sold or decreases in Quality
                if (item.Name != "Sulfuras, Hand of Ragnaros")
                {
                    item.SellIn = item.SellIn - 1;
                }

                if (item.SellIn >= 0) continue;

                if (item.Name != "Aged Brie")
                {
                    if (item.Name.Contains("Backstage passes"))
                    {
                        if (item.Quality <= 0) continue;

                        if (item.Name != "Sulfuras, Hand of Ragnaros")//"Sulfuras", being a legendary item, never has to be sold or decreases in Quality
                        {
                            item.Quality = item.Quality - 1;
                        }
                    }
                    else
                    {
                        item.Quality = item.Quality - 2; //Once the sell by date has passed, Quality degrades twice as fast
                    }
                }
                else
                {
                    if (item.Quality < 50)
                    {
                        item.Quality = item.Quality + 1;
                    }
                }
            }
            Console.WriteLine("Inventory update complete");
        }
    }

    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }
}