using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// 乘车人信息
    /// </summary>
    [Serializable]
    public class Passenger
    {
        private CardType _cardType=CardType.二代身份证;

        [DisplayName("证件类型")]
        public CardType CardType
        {
            get { return _cardType; }
            set { _cardType = value; }
        }

        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("证件号码")]
        public string CardNo { get; set; }


        [DisplayName("手机号码")]
        public string MobileNo { get; set; }

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool Checked { get; set; }

        private SeatType _seatType=SeatType.硬卧;
        /// <summary>
        /// 座位，默认硬卧
        /// </summary>
        public SeatType SeatType
        {
            get { return _seatType; }
            set { _seatType = value; }
        }

        private TicketType _ticketType=TicketType.成人票;

        /// <summary>
        /// 默认成人票
        /// </summary>
        public TicketType TicketType
        {
            get { return _ticketType; }
            set { _ticketType = value; }
        }

        private SeatDetailType _seatDetailType=SeatDetailType.随机;

        /// <summary>
        /// 默认随机,只针对卧铺有效？
        /// </summary>
        public SeatDetailType SeatDetailType
        {
            get { return _seatDetailType; }
            set { _seatDetailType = value; }
        }
    }

    /// <summary>
    ///  全部乘客信息
    /// </summary>
    public class PassengerCollection : List<Passenger>
    {
        
    }
}