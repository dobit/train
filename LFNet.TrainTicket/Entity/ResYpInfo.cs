namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// {"countT":0,"count":5,"ticket":111,"op_1":true,"op_2":false}
    /// </summary>
    public class ResYpInfo
    {
        /// <summary>
        /// ʱ�� �Ŷ�����
        /// </summary>
        public int CountT { get; set; }

        /// <summary>
        /// ���� //�������ж������������ύ��ͬ�Ĺ�Ʊ����
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Ʊ��
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// �����µ���
        /// </summary>
        public bool Op_1 { get; set; }

        /// <summary>
        /// Ŀǰ�Ŷ������Ѿ�������Ʊ����������ѡ������ϯ��򳵴Σ��ش����ѡ�";
        /// </summary>
        public bool Op_2 { get; set; }

    } 
}