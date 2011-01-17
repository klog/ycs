using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Xml.Linq;

namespace YCSLib
{
    public static class YMSGRoomCatalog
    {
        const int MAX_CATEGORY_COUNT = 20;

        static XDocument xCategories;
        static Dictionary<string, XDocument> xRooms =
            new Dictionary<string, XDocument>(MAX_CATEGORY_COUNT);

        public class YMSGRoomCategory
        {
            private List<YMSGRoomCategory> __subCategories = null;
            public string Id { get; set; }
            public string Name { get; set; }
            public List<YMSGRoomCategory> SubCategories
            {
                get
                {
                    if (__subCategories == null)
                        __subCategories = new List<YMSGRoomCategory>();
                    return __subCategories;
                }
                set
                {
                    __subCategories = value;
                }
            }

            public List<YMSGRoom> Rooms
            {
                get
                {
                    if(!xRooms.ContainsKey(this.Id))
                        xRooms[this.Id] = XDocument.Load(string.Format(Resources._1003, "chatroom_" + this.Id));
                    List<YMSGRoom> retVal = new List<YMSGRoom>();
                    YMSGRoom room = null;
                    foreach (var v in xRooms[this.Id].Descendants())
                    {
                        if (v.Name == "room")
                        {
                                if(room !=null)
                                    retVal.Add(room);
                                room = new YMSGRoom()
                                {
                                    Id = v.Attribute("id").Value,
                                    Name = v.Attribute("name").Value,
                                    Type = v.Attribute("type").Value,
                                    Topic = v.Attribute("topic").Value
                                };
                        }

                        if (v.Name == "lobby")
                        {
                            room.RoomCount = v.Attribute("count").Value;
                            room.Users = v.Attribute("users").Value;
                            room.Voices = v.Attribute("voices").Value;
                            room.Webcams = v.Attribute("webcams").Value;
                        }
                    }
                    retVal.Add(room);

                    return retVal;
                }
            }
        }

        public class YMSGRoom
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Topic { get; set; }
            public string RoomCount { get; set; }
            public string Users { get; set; }
            public string Voices { get; set; }
            public string Webcams { get; set; }
        }

        public static List<YMSGRoomCategory> Categories
        {
            get
            {
                if(xCategories == null)
                    xCategories = XDocument.Load(string.Format(Resources._1003, "chatcat"));
                var categories = from item in xCategories.Descendants("category").AsParallel()
                                 where item.Ancestors().Count<XElement>() > 1 && item.Ancestors().Count<XElement>() < 4
                                 select new
                                 {
                                     Id = item.Attribute("id").Value,
                                     Name = item.Attribute("name").Value,
                                     Rank = item.Ancestors().Count<XElement>()
                                 };

                List<YMSGRoomCategory> retVal = new List<YMSGRoomCategory>();
                foreach (var v in categories)
                    if (v.Rank == 2)
                        retVal.Add(new YMSGRoomCategory() { Id = v.Id, Name = v.Name });
                    else
                        retVal[retVal.Count - 1].SubCategories.Add(new YMSGRoomCategory() { Id = v.Id, Name = v.Name });

                return retVal;
            }
        }
    }
}
