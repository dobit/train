﻿namespace LFNet.TrainTicket.Entity
{
    /// <summary>
    /// 座位类型
    /// 1|硬座,2|软座,3|硬卧,4|软卧,6|高级软卧,9|商务座,M|一等座,O|二等座,P|特等座,Q|观光座,S|一等包座
    /// </summary>
    public enum SeatType
    {
        硬座='1',
        软座='2',
        硬卧='3',
        软卧='4',
        高级软卧='6',
        商务座=9,
        一等座='M',
        二等座='O',
        特等座='P',
        观光座='Q',
        一等包座='S',
        无座='W'//未知编码
    }
}