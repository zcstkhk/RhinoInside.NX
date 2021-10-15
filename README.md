[English Description](https://github.com/zcstkhk/RhinoInside.NX/blob/master/README_EN.md)

# RhinoInside.NX

连接 Siemens NX 和 Rhino 的插件

# 目前仍然是处于非常早期的测试版本.

# 注意事项
>1. 要使用此工具，必须使用 Rhino 7 与 NX 1847 及以上版本。
>2. 鉴于编写的 Brep 代码目前仍然问题较多，因此使用 STEP 来进行格式转换，因此要求 NX 有 STEP242 的许可证。
>3. NX 1953 在导入 STEP 的过程中存在问题，请不要使用此版本。

下载最新版本 [点击此处](https://github.com/zcstkhk/RhinoInside.NX/releases/)

提供的程序是 NX 1872 的版本，理论上适用于 1872 及更高版本，如果不适用，可以自己进行编译并进行签名，更多和签名相关的信息，请点击下方的链接。如果有相关问题或者需要提供其它版本，请提交 issue。

[签名过程](https://docs.plm.automation.siemens.com/tdoc/nx/1847/nx_api/#uid:signing_process)

# 使用方法
>1. 安装 Rhino 和 NX.
>2. 打开 RhinoInside.NX Starter.
>3. 根据界面提示进行选择.
>4. 点击启动.

# 编译方法
>1. 安装 NX 时将安装目录设置为如下形式，XXX\Siemens\NXVER，其中 XXX 为任意文件夹，注意最好不要有空格，否则可能会引发其它问题，NXVER 为 NX 的版本号，比如 NX1872、NX1953，注意 NX 和 版本号之间不要有空格。
>2. 创建环境变量 SPLM_ROOT_DIR，指向第一步安装路径中的 XXX\Siemens。
>3. 创建环境变量 RHINO_ROOT_DIR，指向 RHINO 的安装目录，比如 C:\Programs\Rhino\7。

#已知问题
>1. 不能修改 Grasshopper 中组件的描述（名称），否则会卡死，原因暂时未知。
>2. 实际测试的时候发现，NX 1953 导入 STP 的时候可能会有问题，提示 Failed to find/create NX OCC part for PLM part，西门子支持网站上显示可能是一个 bug