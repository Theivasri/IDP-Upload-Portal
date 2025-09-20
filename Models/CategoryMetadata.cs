using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IDPUpload_Portal.Models
{
    public class CategoryMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("MetadataID")]
    public string MetadataId { get; set; }

    [BsonElement("CategoryID")]
    public string CategoryId { get; set; }

    [BsonElement("FieldName")]
    public string FieldName { get; set; }

    [BsonElement("Status")]
    public string Status { get; set; }

    [BsonElement("CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [BsonElement("CreatedBy")]
    public string CreatedBy { get; set; }

    [BsonElement("UpdatedOn")]
    public DateTime UpdatedOn { get; set; }

    [BsonElement("UpdatedBy")]
    public string UpdatedBy { get; set; } // fixed property name
}
}