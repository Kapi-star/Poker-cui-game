using System;
using System.Linq;
using System.Collections.Generic;

class Program
{
	enum Score{
		LoyalStraightFlash = 100000,
		StraightFlash = 20000,
		FourCard = 10000,
		FullHouse = 5000,
		Flash = 4000,
		Straight = 3000,
		ThreeCard = 1100,
		TwoPair = 1000,
		HighCard = 0
	}

	static void Main()
	{
		int totalScore = 0;
		bool DoubleUpFlag = false;
		while(true){
			Console.WriteLine(String.Format("ポーカーを始めます！現在のスコア:{0}", totalScore));
			Console.WriteLine("カードが配られました。どのカードを交換しますか？番号を入力してください。");
			Console.WriteLine();
			
			Random r = new Random();
			int [] cardNumbers = {};
			string [] cardMarks = {};
			for(int i=1; i <= 5; i++){
				int num = r.Next(1, 14);
				Array.Resize(ref cardNumbers, cardNumbers.Length + 1);
				cardNumbers[cardNumbers.Length - 1] = num;

				int markidx = r.Next(4);
				string mark = GetMark(markidx);
				Array.Resize(ref cardMarks, cardMarks.Length + 1);
				cardMarks[cardMarks.Length - 1] = mark;
			}

			var tapleData = dupChange(cardNumbers, cardMarks);
			cardNumbers = tapleData.Item1;
			cardMarks = tapleData.Item2;

			string[] viewCardNumbers = viewCardNumbersGet(cardNumbers);

			var isCardChange = new bool[] {false, false, false, false, false};
			while(true){
				for(int i=0; i < 5; i++){
					if(isCardChange[i]){
						Console.WriteLine(String.Format("({0}: {1}の{2})", i+1, viewCardNumbers[i], cardMarks[i]));
					}else{
						Console.WriteLine(String.Format("{0}: {1}の{2}", i+1, viewCardNumbers[i], cardMarks[i]));
					}
				}
				Console.WriteLine();
				Console.WriteLine("交換するカードを選び終わったら6と入力してください。");

				int changeCardIdx;
				while(true){
					string input = Console.ReadLine();
					if(int.TryParse(input, out changeCardIdx) && 1 <= changeCardIdx && changeCardIdx <= 6){
						break;
					}else{
						Console.WriteLine("1～6の半角数字で入力してください；；");
					}
				}
				if( changeCardIdx == 6){
					break;
				}
				isCardChange[changeCardIdx-1] = !isCardChange[changeCardIdx-1];
			}

			var hashsetNum = new HashSet<int>();
			int[] dupIdx = {};
			var hashsetMark = new HashSet<string>();
			for(int i=0; i < 5; i++){
				if(isCardChange[i]){
					int num = r.Next(1, 14);
					cardNumbers[i] = num;

					int markidx = r.Next(4);
					cardMarks[i] = GetMark(markidx);


					if(!hashsetNum.Add(cardNumbers[i])){
						Array.Resize(ref dupIdx, dupIdx.Length + 1);
						dupIdx[dupIdx.Length - 1] = i+1;
					}
					while(!hashsetMark.Add(cardMarks[i]) && Array.Find(dupIdx, idx => idx-1 == i) != 0){
						int changeNum = r.Next(1, 14);
						cardNumbers[i] = changeNum;
						int changeMarkidx = r.Next(4);
						string changeMark = GetMark(changeMarkidx);
						cardMarks[i] = changeMark;
					}
				}
			}	

			if(!isCardChange.All(val => val == false)){
				Console.WriteLine("カードの交換をしました！");
			}
			Console.WriteLine("あなたのカードの役は…。");

			viewCardNumbers = viewCardNumbersGet(cardNumbers);
			for(int i=0; i < 5; i++){
				Console.WriteLine(String.Format("{0}: {1}の{2}", i+1, viewCardNumbers[i], cardMarks[i]));
			}

			for(int i=0; i < 5; i++){
				if(cardNumbers[i] == 1){
					cardNumbers[i] = 14;
				}
			}
			Score result = checkHand(cardNumbers, cardMarks);
			Console.WriteLine(GetHand(result.ToString()));

			int score = (int)result;
			if(score == 1100){
				score = 1000;
			}
			if(score == 0 && DoubleUpFlag == true){
				Console.WriteLine("ダブルアップ失敗…最終スコア0。また遊んでね！");
				break;
			}else if(DoubleUpFlag == true){
				int doubleUp = GetDouble(result.ToString());
				totalScore = totalScore * doubleUp;
				Console.WriteLine(String.Format("ダブルアップ成功！今回の獲得スコア{0}", totalScore));
			}else if(score == 0){
				Console.WriteLine("残念。また遊んでね！");
				break;
			}else{
				Console.WriteLine(String.Format("今回の獲得スコア{0}", score));
				totalScore += score;
			}

			if(score > 0){
				bool breakFlag = false;
				while(true){
					Console.WriteLine("ダブルアップしますか？次も役をそろえれば役に応じて今回の獲得スコアが数倍またはキープになります。(y/n)");
					string input = Console.ReadLine();
					if(input == "n"){
						Console.WriteLine(String.Format("最終スコア{0}", totalScore));
						breakFlag =true;
						break;
					}else if(input == "y"){
						if(DoubleUpFlag == false){
							DoubleUpFlag = true;
						}
						break;
					}else{
						Console.WriteLine("yかnで入力してください；；");
					}
				}
				if(breakFlag){
					break;
				}
			}
		}
        	
    	}

	static string GetMark(int markidx)
	{
        	switch (markidx)
        	{
            	case 0:
                	return "ハート";
            	case 1:
                	return "クローバー";
           	case 2:
                	return "スペード";
		case 3:
                	return "ダイヤ";
            	default:
                	return "";
        	}
    	}

	static Tuple<int[], string[]> dupChange(int[] cardNumbers, string[] cardMarks){
		int[] dupIdx = {};
			Random r = new Random();
			var hashsetNum = new HashSet<int>();
			for(int i = 0; i < 5; i++){
				if(!hashsetNum.Add(cardNumbers[i])){
					Array.Resize(ref dupIdx, dupIdx.Length + 1);
					dupIdx[dupIdx.Length - 1] = i+1;
				}
			}
			var hashsetMark = new HashSet<string>();
			for(int i = 0; i < 5; i++){
				if(!hashsetMark.Add(cardMarks[i])){
					if(Array.Find(dupIdx, idx => idx-1 == i) != 0){
						int num = r.Next(1, 14);
						cardNumbers[i] = num;
						int markidx = r.Next(4);
						string mark = GetMark(markidx);
						cardMarks[i] = mark;
					}
				}
			}
		return Tuple.Create(cardNumbers, cardMarks);
	}

	static string[] viewCardNumbersGet(int[] cardNumbers){
		string [] viewCardNumbers = new string[cardNumbers.Length];
		for(int i = 0; i < 5; i++){
				if(cardNumbers[i] == 1){
					viewCardNumbers[i] = "A";
				}else if(cardNumbers[i] == 11){
					viewCardNumbers[i] = "J";
				}else if(cardNumbers[i] == 12){
					viewCardNumbers[i] = "Q";
				}else if(cardNumbers[i] == 13){
					viewCardNumbers[i] = "K";
				}else{
					viewCardNumbers[i] = cardNumbers[i].ToString();
				}
			}
		return viewCardNumbers;
	} 

	static Score checkHand(int[] nums, string[] marks)
	{
		bool isFlash = true;
		bool isStraight = true;
		int sameNumCount = 0;
		int sameNum = 0;
		int matchPairCount = 0;

		Array.Sort(nums);

		for(int i=0; i < 5; i++){
			if(i == 0){
				continue;
			}

			int num = nums[i];
			string mark = marks[i];
			int prevNum = nums[i - 1];
			string prevMark = marks[i - 1];

			if(isFlash && mark != prevMark){
				isFlash = false;
			}

			if(isStraight && num != prevNum+1){
				isStraight = false;
			}

			if(num == prevNum){
				sameNumCount++;
				if(i == 4){
					if(sameNumCount == 1){
						matchPairCount++;
					}else{
						sameNum = sameNumCount + 1;
					}
				}
			}else{
				if(sameNumCount == 1){
					matchPairCount++;
				}else if(sameNumCount > 1){
					sameNum = sameNumCount + 1;
				}
				sameNumCount = 0;
			}
		}

		if(isFlash && isStraight) {
			if(nums[0] == 10 && nums[4] == 14){
				return Score.LoyalStraightFlash;
			}else{
				return Score.StraightFlash;
			}
		}else if(sameNum == 4){
			return Score.FourCard;
		}else if(sameNum == 3){
			if(matchPairCount == 1){
				return Score.FullHouse;
			}else{
				return Score.ThreeCard;
			}
		}else if(isFlash){
			return Score.Flash;
		}else if(isStraight){
			return Score.Straight;
		}else if(matchPairCount == 2){
			return Score.TwoPair;
		}else{
			return Score.HighCard;
		}
    	}

	static string GetHand(string key)
	{
		switch (key)
        	{
            	case "LoyalStraightFlash":
                	return "ロイヤルストレートフラッシュ";
            	case "StraightFlash":
                	return "ストレートフラッシュ";
           	case "FourCard":
                	return "フォーカード";
		case "FullHouse":
                	return "フルハウス";
		case "Flash":
			return "フラッシュ";
		case "Straight":
			return "ストレート";
		case "ThreeCard":
			return "スリーカード";
		case "TwoPair":
			return "ツーペア";
            	default:
                	return "役なし";
        	}
	}

	static int GetDouble(string key)
	{
		switch (key)
        	{
            	case "LoyalStraightFlash":
                	return 100;
            	case "StraightFlash":
                	return 20;
           	case "FourCard":
                	return 10;
		case "FullHouse":
                	return 5;
		case "Flash":
			return 4;
		case "Straight":
			return 3;
		case "ThreeCard":
			return 1;
		case "TwoPair":
			return 1;
            	default:
                	return 0;
        	}
	}
}