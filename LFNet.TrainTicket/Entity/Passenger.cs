using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// �˳�����Ϣ
    /// </summary>
    [Serializable]
    public class Passenger
    {
        private CardType _cardType=CardType.�������֤;

        [DisplayName("֤������")]
        public CardType CardType
        {
            get { return _cardType; }
            set { _cardType = value; }
        }

        [DisplayName("����")]
        public string Name { get; set; }
        [DisplayName("֤������")]
        public string CardNo { get; set; }


        [DisplayName("�ֻ�����")]
        public string MobileNo { get; set; }

        /// <summary>
        /// �Ƿ�ѡ
        /// </summary>
        public bool Checked { get; set; }

        private SeatType _seatType=SeatType.Ӳ��;
        /// <summary>
        /// ��λ��Ĭ��Ӳ��
        /// </summary>
        public SeatType SeatType
        {
            get { return _seatType; }
            set { _seatType = value; }
        }

        private TicketType _ticketType=TicketType.����Ʊ;

        /// <summary>
        /// Ĭ�ϳ���Ʊ
        /// </summary>
        public TicketType TicketType
        {
            get { return _ticketType; }
            set { _ticketType = value; }
        }

        private SeatDetailType _seatDetailType=SeatDetailType.���;

        /// <summary>
        /// Ĭ�����,ֻ���������Ч��
        /// </summary>
        public SeatDetailType SeatDetailType
        {
            get { return _seatDetailType; }
            set { _seatDetailType = value; }
        }
    }

    /// <summary>
    ///  ȫ���˿���Ϣ
    /// </summary>
    public class PassengerCollection : List<Passenger>
    {
        
    }
}