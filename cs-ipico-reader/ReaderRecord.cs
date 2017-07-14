using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.Sports.IPICO
{
    public class ReaderRecord
    {
        protected string mTagID="";
        protected DateTime mReaderTime;
        protected int mReaderMilliSeconds;
        protected int mPCMilliseconds;
        protected string mChan1;
        protected string mChan2;
        protected string mDecId;
        protected int mId=-1;
        protected string mFSLS;
        protected string mReaderName = "";

        public string ReaderName
        {
            get { return mReaderName; }
            set { mReaderName = value; }
        }

        public int ID
        {
            get { return mId; }
            set { mId = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is ReaderRecord)
            {
                ReaderRecord rec = obj as ReaderRecord;
                return mTagID == rec.mTagID && mReaderTime == rec.mReaderTime &&
                    mReaderMilliSeconds == rec.mReaderMilliSeconds && mPCMilliseconds == rec.mPCMilliseconds &&
                    mChan1 == rec.mChan1 && mChan2 == rec.mChan2 && mDecId == rec.mDecId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(mTagID, mReaderTime, mReaderMilliSeconds, mPCMilliseconds, mChan1, mChan2, mDecId).GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Tag ID: {0}", mTagID);
            sb.AppendFormat("\nReader Time: {0}", mReaderTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendFormat("\nReader Milliseconds: {0}", mReaderMilliSeconds);
            sb.AppendFormat("\nChan1: {0}", mChan1);
            sb.AppendFormat("\nChan2: {0}", mChan2);
            sb.AppendFormat("\nDecId: {0}", mDecId);

            return sb.ToString();
        }

        public string TagID
        {
            get { return mTagID; }
            set { mTagID = value; }
        }

        public DateTime ReaderTime
        {
            get { return mReaderTime; }
            set { mReaderTime = value; }
        }

        public int ReaderMilliseconds
        {
            get { return mReaderMilliSeconds; }
            set { mReaderMilliSeconds = value; }
        }

        public int PCMilliseconds
        {
            get { return mPCMilliseconds; }
            set { mPCMilliseconds = value; }
        }

        public string Chan1
        {
            get { return mChan1; }
            set { mChan1 = value; }
        }

        public string Chan2
        {
            get { return mChan2; }
            set { mChan2 = value; }
        }

        public string DecId
        {
            get { return mDecId; }
            set { mDecId = value; }
        }

        public string FSLS
        {
            get { return mFSLS; }
            set { mFSLS = value; }
        }

        public static ReaderRecord Parse(string fLine)
        {
            ReaderRecord rec = new ReaderRecord();
            if (rec.ParseLine(fLine))
            {
                return rec;
            }
            return null;
        }

        public ReaderRecord()
        {

        }

        public bool ParseLine(string fLine)
        {
            if (!fLine.StartsWith("aa") || fLine.Length <= 2) return false;
            mTagID = ReaderDecoder.ExtractTagID(fLine);
            if (mTagID == null) return false;

            DateTime? dt = ReaderDecoder.ExtractDateTime(fLine);

            if (dt == null)
            {
                return false;
            }

            mReaderTime = dt.Value;
            
            mReaderMilliSeconds = ReaderDecoder.ExtractReaderMilliSeconds(fLine);
            mPCMilliseconds = ReaderDecoder.ExtractPCMilliSeconds(fLine);
            mChan1 = ReaderDecoder.ExtractChan1(fLine);
            mChan2 = ReaderDecoder.ExtractChan2(fLine);
            mDecId = ReaderDecoder.ExtractDecId(fLine);
            mFSLS = ReaderDecoder.ExtractFSLS(fLine);

            return true;
        }
    }
}
