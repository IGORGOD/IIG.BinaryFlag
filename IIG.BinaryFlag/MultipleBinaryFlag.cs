using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace IIG.BinaryFlag
{
    /// <summary>
    ///     Class that Provides Functional for Creating and Working with Multiple Binary Flag Object
    /// </summary>
    public class MultipleBinaryFlag : IDisposable
    {
        private readonly ConcreteBinaryFlag _concreteFlag;
        protected readonly SafeHandle Handle = new SafeFileHandle(IntPtr.Zero, true);
        protected bool Disposed;

        /// <summary>
        ///     Constructor for Multiple Binary Flag Object
        /// </summary>
        /// <param name="length">Length of Binary Flag - How Many Binaries Does Multiple Binary Flag Contain (2-17179868704) </param>
        /// <param name="initialValue">Initial Value of Binary Elements of Flag</param>
        public MultipleBinaryFlag(ulong length, bool initialValue = true)
        {
            if (length < 2)
                throw new ArgumentOutOfRangeException("Length of Flag must be bigger than 1");
            if (length > 17179868704)
                throw new ArgumentOutOfRangeException("Length of Flag must be lesser than '17179868705'");

            if (length < 33)
                _concreteFlag = new UIntConcreteBinaryFlag(length, initialValue);
            else if (length < 65)
                _concreteFlag = new ULongConcreteBinaryFlag(length, initialValue);
            else
                _concreteFlag = new UIntArrayConcreteBinaryFlag(length, initialValue);
        }

        /// <summary>
        ///     Object Disposing
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Set Flag Value at Given Position to True
        /// </summary>
        /// <param name="position">Position of Binary to Change</param>
        public void SetFlag(ulong position)
        {
            _concreteFlag.SetFlag(position);
        }

        /// <summary>
        ///     Set Flag Value at Given Position to False
        /// </summary>
        /// <param name="position">Position of Binary to Change</param>
        public void ResetFlag(ulong position)
        {
            _concreteFlag.ResetFlag(position);
        }

        /// <summary>
        ///     Get Multiple Binary Flag State
        /// </summary>
        /// <returns>True - if All Binaries are True, otherwise - False</returns>
        public bool? GetFlag()
        {
            return _concreteFlag.GetFlag();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
                Handle.Dispose();

            _concreteFlag.Dispose();

            Disposed = true;
        }

        public override string ToString()
        {
            return _concreteFlag.ToString();
        }

        private abstract class ConcreteBinaryFlag : IDisposable
        {
            protected readonly SafeHandle Handle = new SafeFileHandle(IntPtr.Zero, true);
            protected readonly ulong Length;
            protected bool Disposed;

            protected ConcreteBinaryFlag(ulong length)
            {
                if (length < 2)
                    throw new ArgumentOutOfRangeException("Length of Flag must be bigger than one");
                Length = length;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public virtual void SetFlag(ulong position)
            {
                if (position >= Length)
                    throw new ArgumentOutOfRangeException("Position must be lesser than length");
            }

            public virtual void ResetFlag(ulong position)
            {
                if (position >= Length)
                    throw new ArgumentOutOfRangeException("Position must be lesser than length");
            }

            public abstract bool? GetFlag();

            protected virtual bool? GetFlag(ulong position)
            {
                if (position >= Length)
                    throw new ArgumentOutOfRangeException("Position must be lesser than length");
                return null;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (Disposed)
                    return;

                if (disposing)
                    Handle.Dispose();

                Disposed = true;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (uint i = 0; i < Length; i++)
                {
                    var buf = GetFlag(i);
                    if (buf == null)
                        return null;
                    sb.Append((bool) buf ? 'T' : 'F');
                }

                return sb.ToString();
            }
        }

        private class UIntConcreteBinaryFlag : ConcreteBinaryFlag
        {
            private uint? _flag;

            public UIntConcreteBinaryFlag(ulong length, bool initialValue) : base(length)
            {
                if (Length > 32)
                    throw new ArgumentOutOfRangeException("Length of flag must be lesser than thirty-three");
                _flag = uint.MaxValue;
                if (initialValue)
                    return;
                for (uint i = 0, step = 1; i < Length; i++, step <<= 1)
                    _flag ^= step;
            }

            public override void SetFlag(ulong position)
            {
                if (_flag == null)
                    return;
                base.SetFlag(position);
                _flag |= (uint) (1L << (int) position);
            }

            public override void ResetFlag(ulong position)
            {
                if (_flag == null)
                    return;
                base.ResetFlag(position);
                _flag &= uint.MaxValue ^ (uint) (1L << (int) position);
            }

            public override bool? GetFlag()
            {
                return _flag == null ? (bool?) null : _flag == uint.MaxValue;
            }

            protected override bool? GetFlag(ulong position)
            {
                if (_flag == null)
                    return null;
                base.GetFlag(position);
                return (_flag & (uint) (1L << (int) position)) > 0;
            }

            protected override void Dispose(bool disposing)
            {
                if (Disposed)
                    return;

                if (disposing)
                    Handle.Dispose();

                _flag = null;

                Disposed = true;

                base.Dispose(disposing);
            }
        }

        private class ULongConcreteBinaryFlag : ConcreteBinaryFlag
        {
            private ulong? _flag;

            public ULongConcreteBinaryFlag(ulong length, bool initialValue) : base(length)
            {
                if (Length > 64)
                    throw new ArgumentOutOfRangeException("Length of Flag must be lesser than sixty-five");
                _flag = ulong.MaxValue;
                if (initialValue)
                    return;
                for (ulong i = 0, step = 1; i < Length; i++, step <<= 1)
                    _flag ^= step;
            }

            public override void SetFlag(ulong position)
            {
                if (_flag == null)
                    return;
                base.SetFlag(position);
                _flag |= (ulong) (1L << (int) position);
            }

            public override void ResetFlag(ulong position)
            {
                if (_flag == null)
                    return;
                base.ResetFlag(position);
                _flag &= ulong.MaxValue ^ (ulong) (1L << (int) position);
            }

            public override bool? GetFlag()
            {
                return _flag == null ? (bool?)null : _flag == ulong.MaxValue;
            }

            protected override bool? GetFlag(ulong position)
            {
                if (_flag == null)
                    return null;
                base.GetFlag(position);
                return (_flag & (ulong) (1L << (int) position)) > 0;
            }

            protected override void Dispose(bool disposing)
            {
                if (Disposed)
                    return;

                if (disposing)
                    Handle.Dispose();

                _flag = null;

                Disposed = true;

                base.Dispose(disposing);
            }
        }

        private class UIntArrayConcreteBinaryFlag : ConcreteBinaryFlag
        {
            private uint[] _flag;

            public UIntArrayConcreteBinaryFlag(ulong length, bool initialValue) : base(length)
            {
                if (Length > 17179868704)
                    throw new ArgumentOutOfRangeException("Length of flag must be lesser than '17179868705'");
                _flag = new uint[(int) (Length / 32) + (Length % 32 == 0 ? 0 : 1)];
                var change = initialValue ? 0 : Length;
                for (var i = _flag.Length - 1; i >= 0; i--)
                {
                    _flag[i] = uint.MaxValue;
                    if (initialValue)
                        continue;
                    if (change > 31)
                    {
                        _flag[i] = 0;
                        change -= 32;
                    }
                    else
                    {
                        for (uint j = 0, step = 1; change > 0 && j < 32; j++, change--, step <<= 1)
                            _flag[i] ^= step;
                        change = 0;
                    }
                }
            }

            public override void SetFlag(ulong position)
            {
                if (_flag == null)
                    return;
                base.SetFlag(position);
                _flag[_flag.Length - 1 - (int) (position / 32)] |= (uint) (1L << (int) (position % 32));
            }

            public override void ResetFlag(ulong position)
            {
                if (_flag == null)
                    return;
                base.ResetFlag(position);
                _flag[_flag.Length - 1 - (int) (position / 32)] &= uint.MaxValue ^ (uint) (1L << (int) (position % 32));
            }

            public override bool? GetFlag()
            {
                return _flag?.All(flagPart => flagPart == uint.MaxValue);
            }

            protected override bool? GetFlag(ulong position)
            {
                if (_flag == null)
                    return null;
                base.GetFlag(position);
                return (_flag[_flag.Length - 1 - (int) (position / 32)] & (uint) (1L << (int) (position % 32))) > 0;
            }

            protected override void Dispose(bool disposing)
            {
                if (Disposed)
                    return;

                if (disposing)
                    Handle.Dispose();

                _flag = null;

                Disposed = true;

                base.Dispose(disposing);
            }
        }
    }
}