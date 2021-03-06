# 文件检查器
## 简要介绍

### 主要作用：检查是否缺失某些文件

### 功能组成：

![image-20200702134510026](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702134510026.png)

①：检查数据区：列出需要检查的数据，数据遵循csv格式规则

②：检查结果区：列出检查的结果，这里为显示缺失的文件

③：功能区

​		当前csv：显示当前读取的csv文件的路径

​		当前文件夹：显示当前检查的文件夹

​		文件名格式：设置文件名格式，设置规则如下

```
1 : [n]将被替换为第n列的数据
2 : {}用以表示任意字符
2 : 以0作为起始，而不是1，即第1列实际上为第0列
例如csv文件中的数据为：
AAA,001
BBB,002
CCC,003
...
输入的文件名格式为：[0]-[1]-{}-文件(注意，这里不带文件扩展名)
那么在检查时，将会有如下行为：
把[0]替换为AAA,BBB,CCC等，即用第0列的数据替换[0]，
把[1]替换为001,002,003等，即用第1列的数据替换[1]，
{}的位置表示任意字符
也就是生成如下文件名：
AAA-001-XXX-文件
BBB-002-YYY-文件
CCC-003-ZZZ-文件
...
```

​		选择csv数据：弹出一个打开文件对话框，选择一个csv文件并读取其数据

​		选择文件夹：弹出一个打开文件夹对话框，选择需要检查的目录

​		严格匹配：选择是否启用严格匹配

​		**查询**：点击后会开始比对缺少的文件，并在检查结果区显示，查询按钮左侧会显示 **已有文件数/应有文件数**

​		**提取**：点击后会把"**当前文件夹**"目录下的符合条件的文件（符合文件名格式，且出现在**检查数据区**的文件）复制到该程序同目录下					的"**提取的文件**"文件夹中

### 使用：

#### 直接使用

​		将该程序放在要检查的文件夹中

![image-20200702024720259](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702024720259.png)

​		双击打开该程序

![image-20200702134735575](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702134735575.png)

​		程序会将当前目录文件夹设为打开程序的文件夹，并且默认读取当前目录下的NameID.csv文件（如果该文件存在）

​		输入文件名格式，点击查询或提取进行操作

#### 间接使用

​		打开该程序

​		点击**选择csv数据**，打开一个csv文件并读取

​		点击**选择文件夹**，选择需要检查的文件夹

​		输入文件名格式，点击查询或提取进行操作

### 文件组成

![image-20200702030708944](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702030708944.png)

1. 文件列表检查.exe 

   说明：主程序

2. 提取的文件

   说明：使用提取功能后，符合条件的文件将被复制到该目录下

3. NameID.csv

   说明：如果没有指定要读取的csv文件，则程序默认读取该csv

4. NameCheckerSetting.ini

   说明：储存程序的设置

   ![image-20200702134832436](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702134832436.png)

   Path表示**当前文件夹**的默认位置

   CsvFile表示**当前csv**的默认位置

   NameFormat表示**文件名格式**的默认设置

   IsStrictCheck表示是否启用**严格检查**
   
   可以手动修改，但没必要

## 使用实例

假设我需要搜集班上所有同学的某个文件，并且文件名都是以**姓名-学号-作品名-文件**这种方式命名（假设每个同学都严格按照此规则命名）

现在我需要知道还有谁的文件没有提交，那么可以这样做

### 直接使用

首先我们需要一个csv文件，用来储存本班所有同学的姓名和学号，假设csv的内容如下，第一列为姓名，第二列为学号：

![image-20200702031846466](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702031846466.png)

将这个csv文件命名为NameID.csv，然后将该csv和本程序一同移动到我搜集文件的目录，如下：

![image-20200702135858519](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702135858519.png)

接下来双击该程序，弹出界面如下

![image-20200702032210003](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702032210003.png)

因为是要求用**姓名-学号-作品名-文件**的格式为文件命名，并且在读取的csv文件中，第一列是姓名数据，第二列是学号数据，第三列为任意名字（每个人的作品不同），第四列为固定的文件两个字

那么将**文件名格式**命名为：**[0]-[1]-{}-文件**，然后点击查询

![image-20200702135423637](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702135423637.png)

在检查结果区会显示查询结果，这里会显示缺少了妹弓和佩可的文件，但是在文件列表中，佩可是有文件的

![image-20200702135923131](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702135923131.png)

之所以没筛选出来是由于佩可的文件名不是很规范，不是严格按照**[0]-[1]-{}-文件**的方式命名，要将这类文件也筛选出来，需要将严格匹配取消勾选

![image-20200702135826014](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702135826014.png)

接下来，如果我想把符合条件的文件（即符合文件名格式，且出现在**检查数据区**的文件）提取出来，则可以点击**提取**

将会在该程序的目录下生成一个名为**提取的文件**的文件夹，并将符合条件的文件复制到该文件夹中，如下图：

![image-20200702032850609](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702032850609.png)

### 间接使用

直接打开该程序

![image-20200702140001119](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702140001119.png)

点击**选择csv数据**，读取csv

点击**选择文件夹** ，设置需要检查的文件夹

设置文件名格式

![image-20200702140039107](C:\Users\11717\AppData\Roaming\Typora\typora-user-images\image-20200702140039107.png)

其他操作与直接使用相同

需要注意的是，间接使用的情况下，提取功能的作用相对较为有用

## 注意事项

1：请使用合法的文件夹路径，csv文件，因为该程序并未添加这类异常处理

2：检查数据区的数据可以直接编辑，但是依然推荐以读取csv文件获取数据为主，只有在需要细节上的修改的时候才进行编辑

3：csv的数据列数应大于或等于文件名格式中[n]的数量，该程序并未添加相关的异常处理