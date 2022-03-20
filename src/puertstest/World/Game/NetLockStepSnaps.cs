//
//  文件名称：NetLockStepSnaps.cs
//  简   述：网络帧同步快照
//  创建标识：lrg 2021/8/2
//

using System;
using System.Collections.Generic;
using System.IO;
using GF.Base;
using GF.Game;
using ProtoBuf;
using puertstest.World.Data;
namespace puertstest.World.Game
{
    class NetLockStepSnaps : LockStepSnaps, IDisposable
    {
        private long mRoomId;
        private int mPrevDataFrameId;
        private int mCurrFrameId;
        private int lastDataFrameId;
        private int mMaxFrameId;

        private IList<NetFrameSnap> mFrameSnaps = new List<NetFrameSnap>();


        public void Dispose()
        {
            mFrameSnaps.Clear();
        }


        public void JoinRoom(long roomId, int currFrameId, int prevDataFrameId)
        {
            mRoomId = roomId;
            mCurrFrameId = currFrameId;
            mPrevDataFrameId = prevDataFrameId;
        }


        public void ExitRoom()
        {
            mFrameSnaps.Clear();
            mMaxFrameId = 0;
        }


        /// <summary>
        /// 添加pack
        /// </summary>
        /// <param name="frameData"></param>
        public void AddFrameData(RelayFrameData frameData)
        {
            // if (mRoomId != dto.roomId)
            // {
            //     this.Debug($"房间id对不上:{mRoomId}!={dto.roomId}");
            //     return;
            // }
            if (mPrevDataFrameId != frameData.PrevLoadedFrameId)
            {
                this.Debug($"包不连续:{mPrevDataFrameId}!={frameData.PrevLoadedFrameId}");
                return;
            }
            if (frameData.FrameId > mMaxFrameId)
            {
                mMaxFrameId = frameData.FrameId;
            }
            var snap = new NetFrameSnap(frameData.FrameId, frameData.FramesData);
            mFrameSnaps.Add(snap);
            mPrevDataFrameId = frameData.FrameId;
        }


        /// <summary>
        /// 获得帧字节数组
        /// </summary>
        public IList<NetFrameSnap> frameSnaps => mFrameSnaps;


        public void SaveFrame<T>(T frame)
        {
        }


        public bool HasUnreadFrame()
        {
            return mCurrFrameId <= mMaxFrameId;
        }


        public bool ReadFrame<T>(T frame)
        {
            if (frame is GFrame gFrame)
            {
                if (mFrameSnaps.Count > 0)
                {
                    var snap = mFrameSnaps[0];
                    if (snap.Id == mCurrFrameId)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(snap.Snap))
                        {
                            Serializer.Deserialize(memoryStream, gFrame);
                            gFrame.Deserialize();
                            lastDataFrameId = gFrame.Id;
                        }
                        mFrameSnaps.Remove(snap);
                    } else
                    {
                        gFrame.Id = mCurrFrameId;
                        gFrame.PrevDataFrameId = lastDataFrameId;
                    }
                } else
                {
                    gFrame.Id = mCurrFrameId;
                    gFrame.PrevDataFrameId = lastDataFrameId;
                }
                mCurrFrameId++;
            }
            return true;
        }


        public long RoomId => mRoomId;
    }

    class NetFrameSnap
    {
        public int Id { get; set; }
        public byte[] Snap { get; set; }


        public NetFrameSnap(int id, byte[] snap)
        {
            Id = id;
            Snap = snap;
        }
    }
}
