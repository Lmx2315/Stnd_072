   M S B A M L   `   `   `  ���� C  ?Stnd_072, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Stnd_072Stnd_072      Stnd_072.Приёмник   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:Stnd_072     -     � Title$  Приёмник��$	��350��$	��420��.��]� .+�o� $	��Top=�.@�m� -    m�Name$ cfg_FFT��$��Параметры БПФ��.@�m� -   $ winFFT��$��Тип окна��.@��� -    ��Name$ comboBox_winFFT��$	��120��m� -   $ sizeFFT��$��Размер БПФ��.@��� -    ��Name$ textBox_sizeFFT��$��23�� �� TextWrapping$
 Wrap=�$��16384��$��60��$��Center=�m� -   $ smooth_1��$��Фильтр1�� m� IsCheckable .  m� 	IsChecked .  m� -   $ smooth_2��$��Фильтр2�� .  .  � $��0,0,2,-1q� � ColumnDefinitions �� $	��39* ��� $	��23* ��� $
��348* �.+��� -	   $ textBox_Att0��$��Right=�$
��Left=�$��23��$��	16,51,0,0q�$
 Wrap=�$��31��$	��Top=�$��43��$��2���� -
    ��Name$ label��$��Атт0��$
��Left=�$��	16,25,0,0q�$	��Top=�$��43��$��2���� -   $ textBox_Att1��$��Right=�$
��Left=�$��23��$��12.725,51,0,0q�$
 Wrap=�$��31��$	��Top=�$��43��$��2���� -   $ 
label_Copy��$��Атт1��$
��Left=�$��12.725,25,0,0q�$	��Top=�$��43��$��2���� -   $ textBox_Att2��$��Right=�$
��Left=�$��23��$��73.725,51,0,0q�$
 Wrap=�$��31��$	��Top=�$��43��$��2���� -   $ label_Copy1��$��Атт2��$
��Left=�$��73.725,25,0,0q�$	��Top=�$��43��$��2���� -   $ textBox_Att3��$��Right=�$
��Left=�$��23��$��135.725,51,0,0q�$
 Wrap=�$��31��$	��Top=�$��43��$��2���� -   $ label_Copy2��$��Атт3��$
��Left=�$��135.725,25,0,0q�$	��Top=�$��43��$��2���� -   	 ��Name$	 button��$��отправить��$
��Left=�$��186.725,51,0,0q�$	��Top=�$��64��$��2���� -   $ label_Copy3��$��№ канала��$
��Left=�$��28.725,0,0,0q�$	��Top=�$��66��$��2���� -   
 ��Name$
 checkBox_SPECTR0��$	��FFT��$
��Left=�$��	18,75,0,0q�$	��Top=�$��2���� -   $
 checkBox_TIME0��$
��time��$
��Left=�$��	18,96,0,0q�$	��Top=�$��2���� -   $
 checkBox_SPECTR1��$	��FFT��$
��Left=�$��14.725,75,0,0q�$	��Top=�$��2���� -   $
 checkBox_TIME1��$
��time��$
��Left=�$��14.725,96,0,0q�$	��Top=�$��2���� -   $
 checkBox_SPECTR2��$	��FFT��$
��Left=�$��73.725,75,0,0q�$	��Top=�$��2���� -   $
 checkBox_TIME2��$
��time��$
��Left=�$��73.725,96,0,0q�$	��Top=�$��2���� -   $
 checkBox_SPECTR3��$	��FFT��$
��Left=�$��135.725,75,0,0q�$	��Top=�$��2���� -   $
 checkBox_TIME3��$
��time��$
��Left=�$��135.725,96,0,0q�$	��Top=�$��2���� -   $ textBox_Att0_Copy��$��Right=�$��23��$��17,206,4.275,0q�$
 Wrap=�$��31��$	��Top=�$��2��$y�Hidden=��� -   $ label_Copy4��$��Дин.диап��$
��Left=�$��	9,180,0,0q�$	��Top=�$��61��$��3��$y�Hidden=��� -   $ label_Copy5��$
��Дб��$
��Left=�$��0.725,206,0,0q�$	��Top=�$��27��$��2��$y�Hidden=��� -   $ textBox_SCH_UDP0��$��Right=�$
��Left=�$��23��$��
295,27,0,0q�$
 Wrap=�$��0��$	��Top=�$��43��$��2���� -   $ textBox_SCH_UDP1��$��Right=�$
��Left=�$��23��$��
295,55,0,0q�$
 Wrap=�$��0��$	��Top=�$��43��$��2���� -    $ textBox_SCH_UDP2��$��Right=�$
��Left=�$��23��$��
295,84,0,0q�$
 Wrap=�$��0��$	��Top=�$��43��$��2���� -!   $ textBox_SCH_UDP3��$��Right=�$
��Left=�$��23��$��295,112,0,0q�$
 Wrap=�$��0��$	��Top=�$��43��$��2���� -"   $ label_Copy6��$��Пакеты��$
��Left=�$��
290,1,-8,0q�$	��Top=�$��66��$��2��