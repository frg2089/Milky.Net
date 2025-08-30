using Lagrange.Core;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface.Api;

using Shimakaze.Milky.Model.Api.File;
using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Server.Lagrange;

public class LagrangeFileApiEndpoints(BotContext bot, IFileFetcher fileFetcher) : IMilkyFileApiEndpoints
{
    public async Task<CreateGroupFolderOutput> CreateGroupFolderAsync(CreateGroupFolderInput input, CancellationToken cancellationToken = default)
    {
        var result = await bot.GroupFSCreateFolder((uint)input.GroupId, input.FolderName);
        return new(string.Empty);
    }

    public async Task DeleteGroupFileAsync(DeleteGroupFileInput input, CancellationToken cancellationToken = default)
        => await bot.GroupFSDelete((uint)input.GroupId, input.FileId);

    public Task DeleteGroupFolderAsync(DeleteGroupFolderInput input, CancellationToken cancellationToken = default)
        => bot.GroupFSDeleteFolder((uint)input.GroupId, input.FolderId);

    public async Task<GetGroupFileDownloadUrlOutput> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlInput input, CancellationToken cancellationToken = default)
        => new(new(await bot.FetchGroupFSDownload((uint)input.GroupId, input.FileId)));

    public async Task<GetGroupFilesOutput> GetGroupFilesAsync(GetGroupFilesInput input, CancellationToken cancellationToken = default)
    {
        var result = await bot.FetchGroupFSList((uint)input.GroupId, input.ParentFolderId);
        var files = result
            .OfType<BotFileEntry>()
            .Select(i => new GroupFileEntity(
                input.GroupId,
                i.FileId,
                i.FileName,
                i.ParentDirectory,
                (long)i.FileSize,
                i.UploadedTime,
                i.ExpireTime,
                i.UploaderUin,
                (int)i.DownloadedTimes));
        var folders = result
            .OfType<BotFolderEntry>()
            .Select(i => new GroupFolderEntity(
                input.GroupId,
                i.FolderId,
                i.ParentFolderId,
                i.FolderName,
                i.CreateTime,
                i.ModifiedTime,
                i.CreatorUin,
                (int)i.TotalFileCount));

        return new([.. files], [.. folders]);
    }

    public Task<GetPrivateFileDownloadUrlOutput> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task MoveGroupFileAsync(MoveGroupFileInput input, CancellationToken cancellationToken = default)
        => bot.GroupFSMove((uint)input.GroupId, input.FileId, input.ParentFolderId, input.TargetFolderId);

    public Task RenameGroupFileAsync(RenameGroupFileInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task RenameGroupFolderAsync(RenameGroupFolderInput input, CancellationToken cancellationToken = default)
        => bot.GroupFSRenameFolder((uint)input.GroupId, input.FolderId, input.NewFolderName);

    public async Task<UploadGroupFileOutput> UploadGroupFileAsync(UploadGroupFileInput input, CancellationToken cancellationToken = default)
    {
        await using var stream = await fileFetcher.FetchFileAsync(input.FileUri, cancellationToken);
        byte[] array = new byte[stream.Length];
        await stream.ReadExactlyAsync(array, cancellationToken);

        var fileId = await bot.GroupFSUpload((uint)input.GroupId, new(array, input.FileName), input.ParentFolderId);

        return new(string.Empty);
    }

    public async Task<UploadPrivateFileOutput> UploadPrivateFileAsync(UploadPrivateFileInput input, CancellationToken cancellationToken = default)
    {
        await using var stream = await fileFetcher.FetchFileAsync(input.FileUri, cancellationToken);
        byte[] array = new byte[stream.Length];
        await stream.ReadExactlyAsync(array, cancellationToken);

        var result = await bot.UploadFriendFile((uint)input.UserId, new(array, input.FileName));

        // TODO: The URL cannot be located at this time
        return new(string.Empty);
    }
}
