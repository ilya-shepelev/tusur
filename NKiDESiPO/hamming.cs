public class HammigCode
{
	private static bool statusBuild = false;
	public bool StatusBuild
	{
		get { return statusBuild; }
		set { }
	}

	private string hammingCode;
	public string HammingCode
	{
		get { return hammingCode; }
		set { }
	}

	private static string badHammingCode;
	public static string BadHammingCode
	{
		get { return badHammingCode; }
		set { }
	}

	private static string correctHammingCode;
	public static string CorrectHammingCode
	{
		get { return correctHammingCode; }
		set { }
	}

	private static int positionError;
	public static int PositionError
	{
		get { return positionError; }
		set { }
	}

	private static byte[] dataContainer = new byte[0];
	private static byte[] DataContainer
	{
		get { return dataContainer; }
		set { }
	}

	private static byte[] codeContainer = new byte[0];
	private static byte[] CodeContainer
	{
		get { return codeContainer; }
		set { }
	}

	// Конструктор
	public HammigCode(string Code)
	{
		SetDataContainer(Code);
		byte p = this.GetCountControlBit(dataContainer.Length);
		statusBuild = BuildHammigCode(p);
		if (statusBuild)
			hammingCode = ResultFilling(codeContainer);
	}

	// Подсчет необходимых контрольных бит
	private byte GetCountControlBit(int length)
	{
		byte p = 0;
		while (length + p + 1 > Math.Pow(2, p)) p++;
		return p;
	}

	// Заполнение контейнера входных данных
	private void SetDataContainer(string Data)
	{
		dataContainer = new byte[Data.Length];
		for (int i = 0; i < Data.Length; i++)
			dataContainer[i] = byte.Parse(Data[i].ToString());
	}

	// Резервирование разрядов в контейнере кода
	private void ReservBitsInCodeContainer(byte p)
	{
		codeContainer = new byte[dataContainer.Length + p];
		for (int i = 0; i < p; i++)
			codeContainer[Convert.ToByte(Math.Pow(2, i) - 1)] = 2;
	}


	// Заполнение контейнера кода
	private void SetCodeContainer()
	{
		int position = dataContainer.Length - 1;
		for (int i = 0; i < codeContainer.Length; i++)
			if (codeContainer[i] != 2)
		{
			codeContainer[i] = dataContainer[position];
			position--;
		}
	}

	// Подсчет контрольной суммы
	private static int GetCheckSumm(byte[] container)
	{
		int summ = 0;
		for (byte i = 0; i < codeContainer.Length; i++)
			if (codeContainer[i] == 1)
				summ = summ ^ (i + 1);
		return summ;
	}

	// Внедрение контрольной суммы
	private void SetCheckSumm(byte[] container, int summ, byte p)
	{
		string binSum = Convert.ToString(summ, 2);
		Stack NotNullBitPosition = new Stack();

		for (int i = 0; i < p - binSum.Length; i++)
			NotNullBitPosition.Push(0);
		for (int i = 0; i < binSum.Length; i++)
			NotNullBitPosition.Push(binSum[i]);

		for (int i = 0; i < codeContainer.Length; i++)
			if (codeContainer[i] == 2) codeContainer[i] = byte.Parse(NotNullBitPosition.Pop().ToString());
	}

	// Строим код Хэмминга
	private bool BuildHammigCode(byte p)
	{
		ReservBitsInCodeContainer(p);
		SetCodeContainer();
		int summ = GetCheckSumm(codeContainer);
		SetCheckSumm(codeContainer, summ, p);
		if (GetCheckSumm(codeContainer) == 0)
			return true;
		else
			return false;
	}

	// Заполнение результата
	private static string ResultFilling(byte[] container)
	{
		string str = "";
		Array.Reverse(container);
		for (int i = 0; i < container.Length; i++)
			str += container[i].ToString();
		return str;
	}

	// Внесение ошибки
	public static void InsertError(int digit)
	{
		Array.Reverse(codeContainer);
		InverseBit(codeContainer, digit - 1);
		badHammingCode = ResultFilling(codeContainer);
	}

	// Исправление ошибки
	public static void CorrectingError()
	{
		Array.Reverse(codeContainer);
		positionError = GetCheckSumm(codeContainer);
		InverseBit(codeContainer, positionError - 1);
		correctHammingCode = ResultFilling(codeContainer);
	}

	// Инверсия бита
	private static void InverseBit(byte[] container, int position)
	{
		if (container[position] == 1)
			container[position] = 0;
		else
			container[position] = 1;
	}

	// Освобождение ресурсов
	public static void Dispose()
	{
		statusBuild = false;
		Array.Clear(dataContainer, 0, dataContainer.Length);
		Array.Clear(codeContainer, 0, codeContainer.Length);
	}
}
