using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoLa.Api
{
   public class Reading
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("device_id")]
        public string DeviceId { get; set; } = null!;

        [BsonElement("timestamp")]
        public DateTime TimeStamp { get; set; }

        [BsonElement("power_kw")]
        public double PowerKW { get; set; }
    }
}
