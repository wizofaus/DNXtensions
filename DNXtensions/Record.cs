using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DNXtensions
{
    public class Record<T>
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string Json { get; set; } = "{}";
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset Modified { get; set; } = DateTimeOffset.UtcNow;

        public string Type { get; set; } = typeof(T).Name;

        [NotMapped]
        public T Detail
        {
            get => DNXtensions.Json.Deserialize<T>(Json);
            set => Json = DNXtensions.Json.Serialize(value);
        }
    }
}
