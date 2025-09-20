using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace IDPUpload_Portal.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("CategoryID")]
        public string Category_ID { get; set; }

        [BsonElement("CategoryName")]
        public string CategoryName { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; }

        [BsonElement("UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [BsonElement("UpdatedBy")]
        public string UpdatedBy { get; set; }
    }
}