using System.ComponentModel;

namespace PureCode.WeChat
{
  public enum ErrorDescriptions
  {
    #region 公共

    [Description("系统繁忙，此时请开发者稍候再试")]
    SystemBusy = -1,

    [Description("请求成功")]
    Success = 0,

    [Description("AppSecret 错误或者 AppSecret 不属于这个小程序，请开发者确认 AppSecret 的正确性")]
    IllegalAppSecret = 40001,

    [Description("请确保 grant_type 字段值为 client_credential")]
    GrantTypeError = 40002,

    [Description("不合法的 OpenID ，请开发者确认 OpenID （该用户）是否已关注公众号，或是否是其他公众号的 OpenID")]
    InvalidOpenId = 40003,

    [Description("不合法的 AppID ，请开发者检查 AppID 的正确性，避免异常字符，注意大小写")]
    IllegalAppId = 40013,

    [Description("不合法的 access_token ，请开发者认真比对 access_token 的有效性（如是否过期），或查看是否正在为恰当的公众号调用接口")]
    IllegalAccessToken = 40014,

    [Description("无效的 oauth_code")]
    InvalidOAuthCode = 40029,

    [Description("不合法的 refresh_token")]
    IllegalRefreshToken = 40030,

    [Description("缺少多媒体文件数据")]
    MissMediaFileData = 41005,

    [Description("API 调用太频繁，请稍候再试")]
    ApiCallTimesLimit = 45011,

    #endregion 公共

    [Description("部分参数为空")]
    PartialParametersEmpty = 63001,

    [Description("无效的签名")]
    InvalidSign = 63002,

    #region 公众号

    [Description("不合法的媒体文件类型")]
    IllegalMediaType = 40004,

    [Description("不合法的文件类型")]
    IllegalFileType = 40005,

    [Description("不合法的文件大小")]
    IllegalFileSize = 40006,

    [Description("不合法的媒体文件 id")]
    IllegalMediaId = 40007,

    [Description("不合法的消息类型")]
    IllegalMessageType = 40008,

    [Description("不合法的图片文件大小")]
    IllegalImageSize = 40009,

    [Description("不合法的语音文件大小")]
    IllegalSoundSize = 40010,

    [Description("不合法的视频文件大小")]
    IllegalVideoSize = 40011,

    [Description("不合法的缩略图文件大小")]
    IllegalThumbnailSize = 40012,

    [Description("不合法的菜单类型")]
    IllegalMenuType = 40015,

    [Description("不合法的按钮个数")]
    IllegalMenuButtonCount = 40016,

    [Description("不合法的按钮类型")]
    IllegalMenuButtonType = 40017,

    [Description("不合法的按钮名字长度")]
    IllegalMenuButonNameLength = 40018,

    [Description("不合法的按钮 KEY 长度")]
    IllegalMenuButtonKeyLength = 40019,

    [Description("不合法的按钮 URL 长度")]
    IllegalMenuButonUrlLength = 40020,

    [Description("不合法的菜单版本号")]
    IllegalMenuVision = 40021,

    [Description("不合法的子菜单级数")]
    IllegalSubMenuLevel = 40022,

    [Description("不合法的子菜单按钮个数")]
    IllegalSubMenuButtonCount = 40023,

    [Description("不合法的子菜单按钮类型")]
    IllegalSubMenuButtonType = 40024,

    [Description("不合法的子菜单按钮名字长度")]
    IllegalSubMenuButtonNameLength = 40025,

    [Description("不合法的子菜单按钮 KEY 长度")]
    IllegalSubMenuButtonKeyLength = 40026,

    [Description("不合法的子菜单按钮 URL 长度")]
    IllegalSubMenuButtonUrlLength = 40027,

    [Description("不合法的自定义菜单使用用户")]
    IllegalCustomMenuUser = 40028,

    [Description("不合法的 openid 列表")]
    IllegalOpenIdList = 40031,

    [Description("不合法的 openid 列表长度")]
    IllegalOpenIdListLength = 40032,

    [Description("不合法的请求字符，不能包含 \\uxxxx 格式的字符")]
    IllegalRequestCharactor = 40033,

    [Description("不合法的参数")]
    IllegalRequestParameters = 40035,

    [Description("不合法的请求格式")]
    IllegalRequestFormat = 40038,

    [Description("不合法的 URL 长度")]
    IllegalUrlLength = 40039,

    [Description("无效的url")]
    InvalidUrl = 40048,

    [Description("不合法的分组 id")]
    InvalidGroupId = 40050,

    [Description("分组名字不合法")]
    InvalidGroupName = 40051,

    [Description("删除单篇图文时，指定的 article_idx 不合法")]
    InvalidArticalIndex = 40060,

    [Description("分组名字不合法")]
    IllegalGroupName2 = 40117,

    [Description("media_id 大小不合法")]
    InvalidMediaIdSize = 40118,

    [Description("button 类型错误")]
    ButtonTypeError = 40119,

    [Description("子 button 类型错误")]
    SubButtonTypeError = 40120,

    [Description("不合法的 media_id 类型")]
    IllegalMediaIdType = 40121,

    [Description("无效的appsecret")]
    InvalidAppSecret = 40125,

    [Description("微信号不合法")]
    InvalidWeChatAccount = 40132,

    [Description("不支持的图片格式")]
    ImageFormatNotSupported = 40137,

    [Description("请勿添加其他公众号的主页链接")]
    AddOtherOfficalAccountIsForbidden = 40155,

    [Description("oauth_code已使用")]
    OAuthCodeIsUsed = 40163,

    [Description("缺少 access_token 参数")]
    MissAccessToken = 41001,

    [Description("缺少 appid 参数")]
    MissAppId = 41002,

    [Description("缺少 refresh_token 参数")]
    MissRefreshToken = 41003,

    [Description("缺少 secret 参数")]
    MissAppSecret = 41004,

    [Description("缺少 media_id 参数")]
    MissMediaId = 41006,

    [Description("缺少子菜单数据")]
    MissSubMenuData = 41007,

    [Description("缺少 oauth code")]
    MissOAuthCode = 41008,

    [Description("缺少 openid")]
    MissOpenId = 41009,

    [Description("access_token 超时，请检查 access_token 的有效期，请参考基础支持 - 获取 access_token 中，对 access_token 的详细机制说明")]
    AccessTokenExpired = 42001,

    [Description("refresh_token 超时")]
    RefreshTokenExpired = 42002,

    [Description("oauth_code 超时")]
    OAuthCodeExpired = 42003,

    [Description("用户修改微信密码， accesstoken 和 refreshtoken 失效，需要重新授权")]
    PasswordChanged = 42007,

    [Description("需要 GET 请求")]
    RequireGetRequest = 43001,

    [Description("需要 POST 请求")]
    RequirePostRequest = 43002,

    [Description("需要 HTTPS 请求")]
    RequireHttpsRequest = 43003,

    [Description("需要接收者关注")]
    RequireUserSubscribe = 43004,

    [Description("需要好友关系")]
    RequireBeFriend = 43005,

    [Description("需要将接收者从黑名单中移除")]
    RequireMoveOutBlackList = 43019,

    [Description("多媒体文件为空")]
    MediaFileIsEmpty = 44001,

    [Description("POST 的数据包为空")]
    PostDataIsEmpty = 44002,

    [Description("图文消息内容为空")]
    MessageIsEmpty = 44003,

    [Description("文本消息内容为空")]
    TextIsEmpty = 44004,

    [Description("多媒体文件大小超过限制")]
    MediaSizeLimit = 45001,

    [Description("消息内容超过限制")]
    MessageContentLimit = 45002,

    [Description("标题字段超过限制")]
    TitleLimit = 45003,

    [Description("描述字段超过限制")]
    DescriptionLimit = 45004,

    [Description("链接字段超过限制")]
    LinkLengthLimit = 45005,

    [Description("图片链接字段超过限制")]
    ImageLinkLengthLimit = 45006,

    [Description("语音播放时间超过限制")]
    SoundTimeLimit = 45007,

    [Description("图文消息超过限制")]
    MessageLimit = 45008,

    [Description("接口调用超过限制")]
    ApiCallLimit = 45009,

    [Description("创建菜单个数超过限制")]
    MenuCountLimit = 45010,

    [Description("回复时间超过限制")]
    ReplyExpired = 45015,

    [Description("系统分组，不允许修改")]
    ChangeSystemGroupIsDisabled = 45016,

    [Description("分组名字过长")]
    GroupNameLengthLimit = 45017,

    [Description("分组数量超过上限")]
    GroupCountLimit = 45018,

    [Description("客服接口下行条数超过上限")]
    CustomServiceRecordCountLimit = 45047,

    [Description("创建菜单包含未关联的小程序")]
    MenuContainsUnrelatedMiniProgram = 45064,

    [Description("相同 clientmsgid 已存在群发记录，返回数据中带有已存在的群发任务的 msgid")]
    ClientMessageIdExists = 45065,

    [Description("相同 clientmsgid 重试速度过快，请间隔1分钟重试")]
    ClientMessageIdRetryLimit = 45066,

    [Description("clientmsgid 长度超过限制")]
    ClientMessageIdLengthLimit = 45067,

    [Description("不存在媒体数据")]
    MediaNotExist = 46001,

    [Description("不存在的菜单版本")]
    MenuVisionNotExist = 46002,

    [Description("不存在的菜单数据")]
    MenuDataNotExist = 46003,

    [Description("不存在的用户")]
    UserNotExist = 46004,

    [Description("解析 JSON/XML 内容错误")]
    InvalidJsonXml = 47001,

    [Description("api 功能未授权，请确认公众号已获得该接口，可以在公众平台官网 - 开发者中心页中查看接口权限")]
    UnauthorizedApi = 48001,

    [Description("粉丝拒收消息（粉丝在公众号选项中，关闭了 “ 接收消息 ” ）")]
    UserClosedMessageRecieve = 48002,

    [Description("api 接口被封禁，请登录 mp.weixin.qq.com 查看详情")]
    ForbiddenApi = 48004,

    [Description("api 禁止删除被自动回复和自定义菜单引用的素材")]
    DeleteAutoReplyForbidden = 48005,

    [Description("api 禁止清零调用次数，因为清零次数达到上限")]
    ZeroCleanTimesLimit = 48006,

    [Description("没有该类型消息的发送权限")]
    SendMessageForbidden = 48008,

    [Description("用户未授权该 api")]
    UnauthorizedUserApi = 50001,

    [Description("用户受限，可能是违规后接口被封禁")]
    UserLimited = 50002,

    [Description("用户未关注公众号")]
    Unsubscribe = 50005,

    [Description("参数错误 (invalid parameter)")]
    InvalidParameter = 61451,

    [Description("无效客服账号 (invalid kf_account)")]
    InvalidCustomAccount = 61452,

    [Description("客服帐号已存在 (kf_account exsited)")]
    CustomAccountExists = 61453,

    [Description("客服帐号名长度超过限制 ( 仅允许 10 个英文字符，不包括 @ 及 @ 后的公众号的微信号 )(invalid   kf_acount length)")]
    CustomAccountNameLengthLimit = 61454,

    [Description("客服帐号名包含非法字符 ( 仅允许英文 + 数字 )(illegal character in     kf_account)")]
    InvalidCustomAccountName = 61455,

    [Description("客服帐号个数超过限制 (10 个客服账号 )(kf_account count exceeded)")]
    CustomAccountCountLimit = 61456,

    [Description("无效头像文件类型 (invalid   file type)")]
    InvalidHeadImageType = 61457,

    [Description("系统错误 (system error)")]
    SystemError = 61450,

    [Description("日期格式错误")]
    InvalidDateFormat = 61500,

    [Description("不存在此 menuid 对应的个性化菜单")]
    MenuNotFound = 65301,

    [Description("没有相应的用户")]
    UserNotFound = 65302,

    [Description("没有默认菜单，不能创建个性化菜单")]
    DefaultMenuNotFound = 65303,

    [Description("MatchRule 信息为空")]
    MatchRoleEmpty = 65304,

    [Description("个性化菜单数量受限")]
    CustomMenuCountLimit = 65305,

    [Description("不支持个性化菜单的帐号")]
    CustomMenuNotSupported = 65306,

    [Description("个性化菜单信息为空")]
    CustomMenuIsEmpty = 65307,

    [Description("包含没有响应类型的 button")]
    MissButtonResponseType = 65308,

    [Description("个性化菜单开关处于关闭状态")]
    CustomMenuIsClosed = 65309,

    [Description("填写了省份或城市信息，国家信息不能为空")]
    CountryInfoIsEmpty = 65310,

    [Description("填写了城市信息，省份信息不能为空")]
    CityInfoIsExmpty = 65311,

    [Description("不合法的国家信息")]
    IllegalCountryInfo = 65312,

    [Description("不合法的省份信息")]
    IllegalProvinceInfo = 65313,

    [Description("不合法的城市信息")]
    IllegalCityInfo = 65314,

    [Description("该公众号的菜单设置了过多的域名外跳（最多跳转到 3 个域名的链接）")]
    ExternalDomainCountLimit = 65316,

    [Description("不合法的 URL")]
    IllegalUrl = 65317,

    [Description("无效的签名")]
    InvalidSign2 = 87009,

    [Description("POST 数据参数不合法")]
    IllegalPostData = 9001001,

    [Description("远端服务不可用")]
    RemoteServiceUnreachable = 9001002,

    [Description("Ticket 不合法")]
    IllegalTicket = 9001003,

    [Description("获取摇周边用户信息失败")]
    FailGetUserArround = 9001004,

    [Description("获取商户信息失败")]
    FailGetMerchantInfo = 9001005,

    [Description("获取 OpenID 失败")]
    FailGetOpenId = 9001006,

    [Description("上传文件缺失")]
    MissUploadFile = 9001007,

    [Description("上传素材的文件类型不合法")]
    IllegalUploadFileType = 9001008,

    [Description("上传素材的文件尺寸不合法")]
    IllegalUploadFileSize = 9001009,

    [Description("上传失败")]
    UploadFailed = 9001010,

    [Description("帐号不合法")]
    IllegalAccount = 9001020,

    [Description("已有设备激活率低于 50% ，不能新增设备")]
    ActivedDevicesRateIsLow = 9001021,

    [Description("设备申请数不合法，必须为大于 0 的数字")]
    IllegalDeviceIdApplication = 9001022,

    [Description("已存在审核中的设备 ID 申请")]
    ApplyingDeviceIdExists = 9001023,

    [Description("一次查询设备 ID 数量不能超过 50")]
    QueryDeviceIdCountLimit = 9001024,

    [Description("设备 ID 不合法")]
    IllegalDeviceId = 9001025,

    [Description("页面 ID 不合法")]
    IllegalPageId = 9001026,

    [Description("页面参数不合法")]
    IllegalPageParameters = 9001027,

    [Description("一次删除页面 ID 数量不能超过 10")]
    RemovePageIdCountLimit = 9001028,

    [Description("页面已应用在设备中，请先解除应用关系再删除")]
    DevicePageExists = 9001029,

    [Description("一次查询页面 ID 数量不能超过 50")]
    QueryPageIdCountLimit = 9001030,

    [Description("时间区间不合法")]
    IllegalTimeRange = 9001031,

    [Description("保存设备与页面的绑定关系参数错误")]
    RelationBetweenDeviceAndPageParameterError = 9001032,

    [Description("门店 ID 不合法")]
    IllegalShopId = 9001033,

    [Description("设备备注信息过长")]
    DeviceRemarkLengthLimit = 9001034,

    [Description("设备申请参数不合法")]
    IllegalDeviceApplyParameter = 9001035,

    [Description("查询起始值 begin 不合法")]
    InvalidQueryBeginParameter = 9001036,

    #endregion 公众号

    #region 小程序

    [Description("所传page页面不存在，或者小程序没有发布")]
    PageDoesnotExists = 41030,

    [Description("调用分钟频率受限(目前5000次/分钟，会调整)，如需大量小程序码，建议预生成。")]
    AppCodeMinuteLimit = 45009,

    [Description("生成码个数总和到达最大个数限制")]
    AppCodeLimit = 45029,

    [Description("搜索结果总数超过了1000条")]
    PageSearchLimit = 47101,

    [Description("next_page_info 参数错误")]
    InvalidPageSearchNextParam = 47102,

    [Description("订单无效")]
    InvalidOrder = 89300,

    [Description("递交的页面被sitemap标记为拦截，具体查看errmsg提示。")]
    SearchPageBanned = 40066,

    [Description("pages 中的path参数不存在或为空")]
    SearchPagePathIsEmpty = 40210,

    [Description("pages 当中存在不合法的query，query格式遵循URL标准，即k1=v1&k2=v2")]
    InvalidQuerySringInPages = 40212,

    [Description("pages 不存在或者参数为空")]
    PageNotFound = 40219,

    [Description("http请求包不是合法的JSON")]
    InvalidRequestJson = 47001,

    [Description("每次提交的页面数超过1000（备注：每次提交页面数应小于或等于1000）")]
    SubmitPagesBatchLimit = 47004,

    [Description("当天提交页面数达到了配额上限，请明天再试")]
    SubmitPagesDailyLimit = 47006,

    [Description("小程序的搜索开关被关闭。请访问设置页面打开开关再重试	")]
    SearchPageClosed = 85091,

    [Description("小程序的搜索功能被禁用")]
    SearchPageForbidden = 85083,

    [Description("没有绑定开放平台帐号")]
    NotBindOpenAccount = 89002,

    #endregion 小程序
  }

  public enum HttpMethod
  {
    GET,
    POST
  }
}