# FaceXD_SDK

The SDK for FaceXD and Unity/Native Socket connection

## Unity集成使用说明

### 编译集成

1. 使用 Visual Studio 生成解决方案
2. 在项目根目录的`bin`文件夹（及其子目录）里，找到生成的`FaceXDSDK.dll`和`websocket-sharp.dll`两个文件，并将其拖入到你的Unity工程中

### 发布集成

1. 下载Release的`FaceXDSDK.dll` 和 `websocket-sharp.dll` 拖入Unity工程中


## 使用示例

参考`sample/Program.cs`

在Unity脚本中添加对应的对象和实现。

## 当前XD传输的数据：
* uuid
* isTracked
* headYaw
* headPitch
* headRoll
* eyeX
* eyeY
* eyeLOpen
* eyeROpen
* eyeBrowYL
* eyeBrowYR
* eyeBrowLForm
* eyeBrowRForm
* eyeLSmile
* eyeRSmile
* eyeBrowAngleL
* eyeBrowAngleR
* mouthOpenY
* mouthForm
* mouthU
* bodyAngleX
* bodyAngleY
* bodyAngleZ