using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Infrastructure.Monitoring
{
    public static class QuartzLogViewerExtensions
    {
        //public static void UseQuartzLogViewer(this WebApplication app)
        //{
        //    // ===========================================================
        //    // 🔧 1️⃣ TỰ ĐỘNG TẠO THƯ MỤC LOGS + COPY LOG FILE VÀO ĐÚNG NƠI CHẠY
        //    // ===========================================================
        //    var baseDir = AppContext.BaseDirectory;
        //    var logDir = Path.Combine(baseDir, "logs");

        //    if (!Directory.Exists(logDir))
        //    {
        //        Directory.CreateDirectory(logDir);
        //        Console.WriteLine($"📁 Created logs folder at: {logDir}");
        //    }

        //    // Lấy thư mục logs ở project root (nơi bạn chạy dotnet run)
        //    var projectRoot = Directory.GetCurrentDirectory();
        //    var sourceLogsDir = Path.Combine(projectRoot, "logs");

        //    if (Directory.Exists(sourceLogsDir))
        //    {
        //        var sourceFiles = Directory.GetFiles(sourceLogsDir, "*.log", SearchOption.TopDirectoryOnly);
        //        foreach (var file in sourceFiles)
        //        {
        //            var destFile = Path.Combine(logDir, Path.GetFileName(file));
        //            if (!File.Exists(destFile))
        //            {
        //                File.Copy(file, destFile, overwrite: true);
        //                Console.WriteLine($"✅ Copied log file: {Path.GetFileName(file)} → {destFile}");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"⚠️ No source logs folder found at: {sourceLogsDir}");
        //    }

        //    // ===========================================================
        //    // BASIC AUTH BẢO VỆ TRANG /quartz/logs
        //    // ===========================================================
        //    app.Use(async (context, next) =>
        //    {
        //        if (context.Request.Path.StartsWithSegments("/quartz/logs"))
        //        {
        //            string? authHeader = context.Request.Headers["Authorization"];
        //            if (authHeader == null || !authHeader.StartsWith("Basic "))
        //            {
        //                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Quartz Logs\"";
        //                context.Response.StatusCode = 401;
        //                return;
        //            }

        //            string encoded = authHeader.Substring("Basic ".Length).Trim();
        //            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        //            var parts = decoded.Split(':');
        //            if (parts.Length != 2 || parts[0] != "admin" || parts[1] != "admin123")
        //            {
        //                context.Response.StatusCode = 401;
        //                return;
        //            }
        //        }
        //        await next.Invoke();
        //    });

        //    // ===========================================================
        //    // TRANG DANH SÁCH LOG FILES
        //    // ===========================================================
        //    app.MapGet("/quartz/logs", () =>
        //    {
        //        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        //        if (!Directory.Exists(logDir))
        //            return Results.Content("<h3>No logs directory found.</h3>", "text/html");

        //        var files = Directory.GetFiles(logDir, "quartz-*.log")
        //                             .OrderByDescending(f => f)
        //                             .ToList();

        //        var sb = new StringBuilder();
        //        sb.AppendLine("<html><head>");
        //        sb.AppendLine("<title>Quartz Logs</title>");
        //        sb.AppendLine("<style>");
        //        sb.AppendLine("body{font-family:monospace;background:#111;color:#eee;padding:1rem;}");
        //        sb.AppendLine("a{color:#66ccff;text-decoration:none;}");
        //        sb.AppendLine(".top{position:fixed;top:10px;right:10px;}");
        //        sb.AppendLine("button{background:#333;color:#eee;border:none;padding:6px 10px;border-radius:4px;cursor:pointer;}");
        //        sb.AppendLine("button:hover{background:#444;}");
        //        sb.AppendLine("</style></head><body>");
        //        sb.AppendLine("<h2>Quartz Log Viewer</h2>");
        //        sb.AppendLine("<div class='top'>");
        //        sb.AppendLine("<button onclick='window.location.reload()'>🔄 Refresh</button>");
        //        sb.AppendLine("<button onclick='clearLogs()'>🗑️ Clear Old Logs</button>");
        //        sb.AppendLine("</div>");
        //        sb.AppendLine("<ul>");
        //        foreach (var file in files)
        //        {
        //            var name = Path.GetFileName(file);
        //            sb.AppendLine($"<li><a href='/quartz/logs/{name}'>{name}</a> " +
        //                          $"<a href='/quartz/logs/live/{name}'>[🟢 Live]</a> " +
        //                          $"<a href='/quartz/logs/download/{name}' download>[⬇️ Download]</a></li>");
        //        }
        //        sb.AppendLine("</ul>");
        //        sb.AppendLine(@"<script>
        //            async function clearLogs(){
        //                if(!confirm('Are you sure you want to delete all logs?'))return;
        //                const res=await fetch('/quartz/logs/clear',{method:'DELETE'});
        //                alert(await res.text());
        //                location.reload();
        //            }
        //        </script>");
        //        sb.AppendLine("</body></html>");
        //        return Results.Content(sb.ToString(), "text/html");
        //    });

        //    // ===========================================================
        //    // DOWNLOAD FILE LOG
        //    // ===========================================================
        //    app.MapGet("/quartz/logs/download/{filename}", (string filename) =>
        //    {
        //        var logPath = Path.Combine(AppContext.BaseDirectory, "logs", filename);
        //        if (!File.Exists(logPath))
        //            return Results.NotFound("File not found");
        //        var bytes = File.ReadAllBytes(logPath);
        //        return Results.File(bytes, "text/plain", filename);
        //    });

        //    // ===========================================================
        //    // CLEAR LOG FILES
        //    // ===========================================================
        //    app.MapDelete("/quartz/logs/clear", () =>
        //    {
        //        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        //        if (!Directory.Exists(logDir)) return Results.NotFound("Logs folder not found");
        //        var files = Directory.GetFiles(logDir, "quartz-*.log");
        //        foreach (var file in files)
        //        {
        //            try { File.Delete(file); } catch { }
        //        }
        //        return Results.Ok($"Deleted {files.Length} logs.");
        //    });

        //    // ===========================================================
        //    // HIỂN THỊ LOG + SEARCH
        //    // ===========================================================
        //    app.MapGet("/quartz/logs/{filename}", async (string filename) =>
        //    {
        //        var logPath = Path.Combine(AppContext.BaseDirectory, "logs", filename);
        //        if (!File.Exists(logPath)) return Results.NotFound("Log file not found");
        //        var text = await File.ReadAllTextAsync(logPath, Encoding.UTF8);

        //        var html = $@"
        //        <html><head>
        //        <title>{filename}</title>
        //        <meta http-equiv='refresh' content='30'>
        //        <style>
        //            body{{background:#111;color:#eee;font-family:monospace;padding:1rem;}}
        //            pre{{white-space:pre-wrap;line-height:1.4;}}
        //            input{{background:#222;color:#eee;border:1px solid #444;padding:6px;border-radius:4px;width:250px;}}
        //            mark{{background:#ff0;color:#000;}}
        //            .top{{position:fixed;top:10px;right:10px;}}
        //            button{{background:#333;color:#eee;border:none;padding:6px 10px;border-radius:4px;cursor:pointer;}}
        //            button:hover{{background:#444;}}
        //        </style></head>
        //        <body>
        //        <div class='top'>
        //            <a href='/quartz/logs'>⬅ Back</a> |
        //            <a href='/quartz/logs/live/{filename}'>🟢 Live</a> |
        //            <a href='/quartz/logs/download/{filename}'>⬇️ Download</a>
        //        </div>
        //        <h3>{filename}</h3>
        //        <input type='text' id='searchInput' placeholder='🔍 Search keyword...' onkeyup='filterLogs()'/>
        //        <span id='matchCount'></span>
        //        <pre id='logContent'>{System.Net.WebUtility.HtmlEncode(text)}</pre>
        //        <script>
        //            function filterLogs(){{
        //                var input=document.getElementById('searchInput').value.toLowerCase();
        //                var log=document.getElementById('logContent');
        //                var lines=log.innerText.split('\\n');
        //                var filtered=[];var count=0;
        //                for(let line of lines){{
        //                    if(line.toLowerCase().includes(input)){{
        //                        count++;
        //                        filtered.push(line.replace(new RegExp(input,'gi'),m=>'<mark>'+m+'</mark>'));
        //                    }}
        //                }}
        //                log.innerHTML=filtered.join('\\n');
        //                document.getElementById('matchCount').innerText=count+' matches';
        //            }}
        //        </script>
        //        </body></html>";
        //        return Results.Content(html, "text/html");
        //    });

        //    // ===========================================================
        //    // LIVE VIEW (TAIL MODE)
        //    // ===========================================================
        //    app.MapGet("/quartz/logs/live/{filename}", (string filename) =>
        //    {
        //        var html = $@"
        //        <html><head>
        //        <title>Live Log - {filename}</title>
        //        <style>
        //            body{{background:#111;color:#eee;font-family:monospace;padding:1rem;}}
        //            pre{{white-space:pre-wrap;line-height:1.4;max-height:90vh;overflow-y:auto;}}
        //            .controls{{margin-bottom:10px;}}
        //            button{{background:#333;color:#eee;border:none;padding:6px 10px;border-radius:4px;cursor:pointer;}}
        //            button:hover{{background:#444;}}
        //        </style></head><body>
        //        <div class='controls'>
        //            <a href='/quartz/logs/{filename}'>⬅ Back</a> |
        //            <button id='pauseBtn' onclick='togglePause()'>⏸ Pause</button>
        //            <button onclick='scrollBottom()'>⬇️ Scroll</button>
        //        </div>
        //        <pre id='logContent'>Loading...</pre>
        //        <script>
        //            let lastSize=0; let paused=false;
        //            async function fetchLog(){{
        //                if(paused) return;
        //                const res=await fetch('/quartz/logs/tail/{filename}?from='+lastSize);
        //                if(res.ok){{
        //                    const data=await res.text();
        //                    if(data.length>0){{
        //                        document.getElementById('logContent').innerHTML+=data;
        //                        lastSize+=data.length;
        //                        window.scrollTo(0,document.body.scrollHeight);
        //                    }}
        //                }}
        //            }}
        //            function togglePause(){{
        //                paused=!paused;
        //                document.getElementById('pauseBtn').innerText=paused?'▶ Resume':'⏸ Pause';
        //            }}
        //            function scrollBottom(){{
        //                window.scrollTo(0,document.body.scrollHeight);
        //            }}
        //            setInterval(fetchLog,3000);
        //            fetchLog();
        //        </script>
        //        </body></html>";
        //        return Results.Content(html, "text/html");
        //    });

        //    // ===========================================================
        //    // API: TRẢ VỀ PHẦN LOG MỚI TỪ OFFSET
        //    // ===========================================================
        //    app.MapGet("/quartz/logs/tail/{filename}", async (string filename, long? from) =>
        //    {
        //        var logPath = Path.Combine(AppContext.BaseDirectory, "logs", filename);
        //        if (!File.Exists(logPath)) return Results.NotFound("File not found");

        //        var fi = new FileInfo(logPath);
        //        if (from.HasValue && from.Value < fi.Length)
        //        {
        //            using var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //            fs.Seek(from.Value, SeekOrigin.Begin);
        //            using var reader = new StreamReader(fs, Encoding.UTF8);
        //            var text = await reader.ReadToEndAsync();
        //            return Results.Text(System.Net.WebUtility.HtmlEncode(text));
        //        }

        //        return Results.Text(""); // nothing new
        //    });
        //}
    }
}
