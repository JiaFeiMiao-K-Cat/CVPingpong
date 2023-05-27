# CVPingpong

视频: [BV1rM4y1v7Wo](https://www.bilibili.com/video/BV1rM4y1v7Wo)

机械结构参考自: [The Quest To Get A Machine To Juggle](http://www.electrondust.com/2017/10/28/the-quest-to-get-a-machine-to-juggle/)

机器视觉部分基于OpenCVSharp实现， 采用[颜色追踪法](https://blog.csdn.net/GuoZhen_Zhou/article/details/114836660)实现乒乓球位置检测(需要提前设置掩模, 可在`program.cs`中启用`#define CONFIG_CAMERA`). 

机械部分采用ESP-12F作为控制器直接控制舵机并与主机通信.
