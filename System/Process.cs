using System;
using System.IO;
using System.Text;

namespace NexumOS.System
{
    public class Process
    {
        private byte[] _data;
        private byte[] _keyboardEvent;

        private bool running = true;
        private uint pointer = 0;

        public Process(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);

            int SplitResult = FindSequenceIndex(data, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x01 });

            if (SplitResult == -1) { _data = data; }
            else
            {
                _data = new byte[SplitResult];
                Array.Copy(data, 0, _data, 0, SplitResult);

                _keyboardEvent = new byte[data.Length - (SplitResult + 8)];
                Array.Copy(data, SplitResult + 8, _keyboardEvent, 0, _keyboardEvent.Length);
            }
        }

        public void Execute()
        {
            if(!running) return;

            if(pointer != 0)
                pointer++;

            if (pointer == _data.Length)
            {
                Stop();
                return;
            }

            if (_data[pointer] == 0x03) // console command
            {
                if (_data[pointer + 1] == 0x01) // clear console
                {
                    // Console.Clear();
                    pointer += 1;
                }
                if (_data[pointer + 1] == 0x02) // set character at a certain position
                {
                    Console.SetCursorPosition((int)_data[pointer + 2] - 1, (int)_data[pointer + 3] - 1);
                    Console.Write(Encoding.ASCII.GetChars(_data)[pointer + 4]);
                    pointer += 4;
                }
            }
        }

        public void Stop()
        {
            running = false;
            Console.WriteLine();
            Console.WriteLine("-- End of Code --");
        }

        public void PrintCode()
        {
            Console.WriteLine("Code:");

            foreach (byte b in _data)
            {
                Console.Write(b.ToString("X2") + " ");
            }

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Keyboard Event:");
            Console.WriteLine();
            foreach (byte b in _keyboardEvent)
            {
                Console.Write(b.ToString("X2") + " ");
            }
        }

        private int FindSequenceIndex(byte[] source, byte[] sequence)
        {
            for (int i = 0; i < source.Length - sequence.Length + 1; i++)
            {
                bool found = true;

                for (int j = 0; j < sequence.Length; j++)
                {
                    if (source[i + j] != sequence[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return i;
                }
            }

            return -1; // Sequence not found
        }
    }
}
