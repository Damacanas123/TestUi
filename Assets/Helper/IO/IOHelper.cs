using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Slime.Helper.IO
{
    public class IOHelper
    {
        private struct WriteQueueItem
        {
            public string filePath;
            public string content;
            public FileMode fileMode;
        }

        private ConcurrentQueue<WriteQueueItem> _writeQueue = new ConcurrentQueue<WriteQueueItem>();

        public IOHelper()
        {
            Util.ThreadStart(DequeueThread);
        }

        private void DequeueThread()
        {
            while (true)
            {
                if (_writeQueue.TryDequeue(out var queueItem))
                {
                    if (queueItem.fileMode == FileMode.Create)
                    {
                        File.WriteAllText(queueItem.filePath,queueItem.content);    
                    }
                    else
                    {
                        File.AppendAllText(queueItem.filePath,queueItem.content);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public void Write(string filePath, string content)
        {
            _writeQueue.Enqueue(new WriteQueueItem()
            {
                filePath = filePath,
                content = content,
                fileMode = FileMode.Create
            });
        }

        public void Append(string filePath, string content)
        {
            _writeQueue.Append(new WriteQueueItem()
            {
                filePath = filePath,
                content = content,
                fileMode = FileMode.Append
            });
        }

        public string Read(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.UTF8);
        }
    }
}