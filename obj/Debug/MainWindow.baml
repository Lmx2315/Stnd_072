   M S B A M L   `   `   `  ����C  ?Stnd_072, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Stnd_072Stnd_072  5   	       Stnd_072.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Stnd_072     5   	     � WindowStartupLocation$  Manual=�5   	    � Title$ 
MainWindow��5	   	   $	��520��6   $	��420��6)   .��5      ]� .+�5   
   o� $	��Top=�6   .@�5      m� $��_File��6   .@�5      m� -   5         m�Name$ Конфигурация��5      $��_Конфигурация��61   6b   m� 5      $��_Exit��6   5      m� 5      $��_Tools��6   .@�5      m� -   5        $I��C_Загрузка прошивки микроконтроллера��5      5      m� 5      -   5        $)��#_Загрузка кода в SDRAM��5      5      m� 5      -   5        $1��+_Загрузка прошивки ПЛИС��5       m� IsCheckable .  m� 	IsChecked . 6d   5      m� 5      -   5        $' !Панель_управления��5      $(��"_Панели управления��62   .@�5      m� -   5        $+ %Панель_тестирования��5      $,��&_Панель тестирования��68    .  .  6�   m� 5      -   5        $- 'Панель_инициализации��5      $.��(_Панель Инициализации��69    .  .  6�   m� 5      -   5        $) #Панель_синтезатора��5      $*��$_Панель Синтезатора��67    .  .  6�   m� 5      -	   5        $% Панель_приёмника��5      $&�� _Панель Приёмника��65    .  .  6�   m� 5      -
   5        $' !Панель_калибровки��5      $(��"_Панель Калибровки��66    .  .  6�   m� 5       -   5        $! Панель_консоли��5       $"��_Панель Консоли��63    .  .  6�   5!      5#      � 5%   
   .+�5&      �� -   5         ��Name$ textBox_IP_072��5&      $��Right=�60   $
��Left=�6T   $��23��6o   $��
298,36,0,0q�6{    �� TextWrapping$
 Wrap=�6�   $��	127.0.0.1��6�   $	��Top=�6�   $��96��6�    �� 
AcceptsTab . $y�Hidden=�6�   �� 5'      -   5        $ textBox_Port_072��5'      $��Right=�60   $
��Left=�6S   $��23��6n   $��
298,71,0,0q�6z   $
 Wrap=�6�   $	��703��6�   $	��Top=�6�   $��96��6�   $y�Hidden=�6�   �� 5(      -   5        $ textBox_SYSTIME��5(      $��Right=�61   $
��Left=�6U   $��23��6p   $��	36,30,0,0q�6|   $
 Wrap=�6�   $��0��6�   $	��Top=�6�   $	��120��6�    . 6�   �� 5)      -   5         ��Name$ label��5)      $#��Системное время��6#   $
��Left=�6=   $��36,4,0,0q�6X   $	��Top=�6j   $	��120��6�   �� 5*      -   5        	 ��Name$	 button_SYSTIME_SETUP��5*      $
��SYNC��63   $
��Left=�6B   $��
169,30,0,0q�6]   $	��Top=�6q   $��44��6�   6�   �� 5+      -   5        $	 button_SYS_START��5+      $��SERVER��6/   $
��Left=�6@   $��306,298,0,0q�6[   $	��Top=�6p   $��73��6�   
 � ToolTip$:
 4Команда общей инициализации��6�   6�   �� 5-      -   5         ��Name$ checkBox_ch1��5-      $	��К1��6-   $
��Left=�6:   $��	23,95,0,0q�6U   $	��Top=�6h    d� 	IsChecked$
 TrueY�6�   �� 5.      -   5        $ checkBox_ch2��5.      $	��К2��6-   $
��Left=�6:   $��	66,95,0,0q�6U   $	��Top=�6h   $
 TrueY�6�   �� 5/      -   5        $ checkBox_ch3��5/      $	��К3��6-   $
��Left=�6:   $��
109,95,0,0q�6U   $	��Top=�6i   $
 TrueY�6�   �� 50      -   5        $ checkBox_ch4��50      $	��К4��6-   $
��Left=�6:   $��
152,95,0,0q�6U   $	��Top=�6i   $
 TrueY�6�   �� 51      -   5        $ label_ch��51      $'��!Работа/калибровка��6-   $
��Left=�6I   $��	38,66,0,0q�6d   $	��Top=�6w   $��24��6�   �� 52      -   5        $	 button��52      $��START��6%   $
��Left=�65   $��306,260,0,0q�6P   $	��Top=�6e   $��73��6}   $2
 ,Команда запроса статуса��6�    J� RenderTransformOrigin$ 0.493,-0.091&�6�   �� 53      -   5        $ 
label_Copy��53      $*��$адрес сетевой карты��6(   $
��Left=�6F   $��	284,5,0,0q�6a   $	��Top=�6t   $	��128��6�   $y�Hidden=�6�   �� 54      -   5        $ textBox_IP_dest��54      $��Right=�61   $
��Left=�6U   $��23��6p   $��298,120,0,0q�6|   $
 Wrap=�6�   $��	127.0.0.1��6�   $	��Top=�6�   $��96��6�    . $y�Hidden=�6�   �� 55      -   5        $ textBox_Port_dest��55      $��Right=�61   $
��Left=�6T   $��23��6o   $��298,155,0,0q�6{   $
 Wrap=�6�   $��77��6�   $	��Top=�6�   $��96��6�   $y�Hidden=�6�   �� 56      -   5        $ label_Copy1��56      $��адрес 072��6)   $
��Left=�6=   $��
314,95,0,0q�6X   $	��Top=�6l   $��65��6�   $y�Hidden=�6�   �� 57      -   5        $	 button_DAC0��57      $
��DAC0��6*   $
��Left=�69   $��
306,30,0,0q�6T   $	��Top=�6h   $��73��6�   $:
 4Команда общей инициализации��6�   $��29��6�   �� 58      -   5        $	 button_DAC1��58      $
��DAC1��6*   $
��Left=�69   $��
306,64,0,0q�6T   $	��Top=�6h   $��73��6�   $:
 4Команда общей инициализации��6�   $��29��6�   �� 59      -   5        $	 button_ADC0��59      $
��ADC0��6*   $
��Left=�69   $��
306,99,0,0q�6T   $	��Top=�6h   $��73��6�   $:
 4Команда общей инициализации��6�   $��29��6�   �� 5:      -   5        $	 button_ADC1��5:      $
��ADC1��6*   $
��Left=�69   $��306,133,0,0q�6T   $	��Top=�6i   $��73��6�   $:
 4Команда общей инициализации��6�   $��29��6�   �� 5;      -    5        $	 button_FPGA��5;      $
��FPGA��6*   $
��Left=�69   $��306,167,0,0q�6T   $	��Top=�6i   $��73��6�   $:
 4Команда общей инициализации��6�   $��29��6�   �� 5<      -!   5        $	 button_BOARD��5<      $��BOARD��6+   $
��Left=�6;   $��306,201,0,0q�6V   $	��Top=�6k   $��73��6�   $:
 4Команда общей инициализации��6�   $��29��6�   �� 5=      -"   5        $	 button_calibr_ch_ok��5=      $��OK��62   $
��Left=�6?   $��
192,89,0,0q�6Z   $	��Top=�6n   $��32��6�   $2
 ,Команда запроса статуса��6�   6�   �� 5>      -#   5        $ textBox_ATT0��5>      $��Right=�6,   $
��Left=�6O   $��23��6j   $��
42,165,0,0q�6v   $
 Wrap=�6�   $��31��6�   $	��Top=�6�   $��44��6�   $y�Hidden=�6�   �� 5?      -$   5        $ label_FREQ_Copy17��5?      $
��Дб��6/   $
��Left=�6<   $��
90,163,0,0q�6W   $	��Top=�6k   $y�Hidden=�6�   �� 5@      -%   5        $ label_Copy19��5@      $"��Вх. аттенюатор 0��6*   $
��Left=�6E   $��
10,136,0,0q�6`   $	��Top=�6t   $y�Hidden=�6�   �� 5A      -&   5        $ textBox_ATT2��5A      $��Right=�6,   $
��Left=�6O   $��23��6j   $��
42,266,0,0q�6v   $
 Wrap=�6�   $��31��6�   $	��Top=�6�   $��44��6�   $y�Hidden=�6�   �� 5B      -'   5        $ label_FREQ_Copy18��5B      $
��Дб��6/   $
��Left=�6<   $��
90,264,0,0q�6W   $	��Top=�6k   $y�Hidden=�6�   �� 5C      -(   5        $ label_Copy20��5C      $"��Вх. аттенюатор 2��6*   $
��Left=�6E   $��
14,240,0,0q�6`   $	��Top=�6t   $y�Hidden=�6�   �� 5D      -)   5        $ textBox_ATT1��5D      $��Right=�6,   $
��Left=�6O   $��23��6j   $��
42,217,0,0q�6v   $
 Wrap=�6�   $��31��6�   $	��Top=�6�   $��44��6�   $y�Hidden=�6�   �� 5E      -*   5        $ label_FREQ_Copy24��5E      $
��Дб��6/   $
��Left=�6<   $��
92,217,0,0q�6W   $	��Top=�6k   $y�Hidden=�6�   �� 5F      -+   5        $ label_Copy26��5F      $"��Вх. аттенюатор 1��6*   $
��Left=�6E   $��
12,190,0,0q�6`   $	��Top=�6t   $y�Hidden=�6�   �� 5G      -,   5        $ textBox_ATT3��5G      $��Right=�6,   $
��Left=�6O   $��23��6j   $��
42,321,0,0q�6v   $
 Wrap=�6�   $��31��6�   $	��Top=�6�   $��44��6�   $y�Hidden=�6�   �� 5H      --   5        $ label_FREQ_Copy25��5H      $
��Дб��6/   $
��Left=�6<   $��
90,314,0,0q�6W   $	��Top=�6k   $y�Hidden=�6�   �� 5I      -.   5        $ label_Copy27��5I      $"��Вх. аттенюатор 3��6*   $
��Left=�6E   $��
14,296,0,0q�6`   $	��Top=�6t   $y�Hidden=�6�   �� 5J      -/   5        $	 button_att_Copy��5J      $��OK��6.   $
��Left=�6;   $��
44,356,0,0q�6V   $	��Top=�6j   $��42��6�   $>
 8Команда установки аттенюатора��6�   $y�Hidden=�6�   5L      5O      5Q      