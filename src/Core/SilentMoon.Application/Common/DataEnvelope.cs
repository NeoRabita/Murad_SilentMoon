using System.Collections.Generic;

namespace SilentMoon.Application.Common
{
    public class DataEnvelope<T>
    {
        public List<T> Data { get; set; }

        public DataEnvelope()
        {
        }

        public DataEnvelope(List<T> data)
        {
            Data = data;
        }
    }
}
