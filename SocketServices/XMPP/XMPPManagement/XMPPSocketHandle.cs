

// ReSharper disable CheckNamespace

namespace RadiantConnect.SocketServices.XMPP.XMPPManagement
{
	public class XMPPSocketHandle(Stream? incomingStream, Stream outgoingStream) : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		internal bool DoBreak;
		internal delegate void InternalMessage(string data);

		internal event InternalMessage? OnClientMessage;
		internal event InternalMessage? OnServerMessage;

		public void Dispose()
		{
			_cancellationTokenSource.Cancel();
			try { incomingStream?.Dispose(); } catch { /* ignored */ }
			try { outgoingStream.Dispose(); } catch { /* ignored */ }
			_cancellationTokenSource.Dispose();
			GC.SuppressFinalize(this);
		}

		internal void Initiate()
		{
			Task.Run(IncomingHandler);
			Task.Run(OutgoingHandler);
		}

		internal async Task IncomingHandler()
		{
			try
			{
				int byteCount;
				byte[] bytes = new byte[8192];
				do
				{
					if (incomingStream is null) break;
					byteCount = await incomingStream.ReadAsync(bytes, _cancellationTokenSource.Token).ConfigureAwait(false);
					string content = Encoding.UTF8.GetString(bytes, 0, byteCount);
					await outgoingStream.WriteAsync(bytes.AsMemory(0, byteCount), _cancellationTokenSource.Token).ConfigureAwait(false);
					Array.Clear(bytes);
					OnClientMessage?.Invoke(content);
				} while (byteCount != 0 && !DoBreak);
			}
			catch (OperationCanceledException) {/**/}
			catch (IOException)
			{
				DoBreak = true;
			}
		}

		internal async Task OutgoingHandler()
		{
			try
			{
				int byteCount;
				byte[] bytes = new byte[8192];
				do
				{
					if (incomingStream is null) break;
					byteCount = await outgoingStream.ReadAsync(bytes, _cancellationTokenSource.Token).ConfigureAwait(false);
					string content = Encoding.UTF8.GetString(bytes, 0, byteCount);
					await incomingStream.WriteAsync(bytes.AsMemory(0, byteCount), _cancellationTokenSource.Token).ConfigureAwait(false);
					Array.Clear(bytes);
					OnServerMessage?.Invoke(content);
				} while (byteCount != 0 && !DoBreak);
			}
			catch (OperationCanceledException) {/**/}
			catch (IOException)
			{
				DoBreak = true;
			}
		}

		public async Task SendXmlMessageAsync([StringSyntax(StringSyntaxAttribute.Xml)] string data)
		{
			if (_cancellationTokenSource.IsCancellationRequested) return;

			try
			{
				while (!incomingStream?.CanWrite ?? false)
				{
					if (incomingStream is null) break;
					await Task.Delay(50).ConfigureAwait(false);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(data);

				if (incomingStream is null) return;
				await incomingStream.WriteAsync(bytes.AsMemory(0, bytes.Length), _cancellationTokenSource.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException) {/**/}
			catch (Exception ex) { Debug.WriteLine(ex); }
		}

		public async Task SendXmlToOutgoingStream([StringSyntax(StringSyntaxAttribute.Xml)] string data)
		{
			try
			{
				while (!outgoingStream.CanWrite) await Task.Delay(50).ConfigureAwait(false);
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				await outgoingStream.WriteAsync(bytes.AsMemory(0, bytes.Length), _cancellationTokenSource.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException) {/**/}
			catch (Exception ex) { Debug.WriteLine(ex); }
		}
	}
}
