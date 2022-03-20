namespace TnyFramework.Net.DotNetty.Codec
{
    public class DatagramPackDecodeMarker
    {
        private byte option = 0;

        private int payloadLength = 0;

        private bool mark = false;
        
        public DatagramPackDecodeMarker()
        {
            
        }
        
        /// <summary>
        ///  是否记录
        /// </summary>
        public bool Mark => mark;

        /// <summary>
        /// 选项
        /// </summary>
        /// <returns></returns>
        public byte Option() => option;

        /// <summary>
        /// 负载长度
        /// </summary>
        /// <returns></returns>
        public int PayloadLength() => payloadLength;


        internal void Record(byte optionValue, int payloadLengthValue)
        {
            option = optionValue;
            payloadLength = payloadLengthValue;
            mark = true;
        }


        internal void Reset()
        {
            option = 0;
            payloadLength = 0;
            mark = false;
        }
    }
}
