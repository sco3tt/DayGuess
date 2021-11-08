using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DayGuess.Components;

namespace DayGuess.Models
{

	public class GameManager
	{
		public int GameCount { get; internal set; } = 0;
		public int TotalGuesses { get; internal set; } = 0;
		public int CorrectGuesses { get; internal set; } = 0;
		public DateTime Date { get; internal set; }

		public DateTime MinDate { get; internal set; }
		public DateTime MaxDate { get; internal set; }

		private int range = 0;
		private GameContainer container = null;

		public DateTime? StartTime { get; internal set; } = null;
		public DateTime? WinTime { get; internal set; } = null;
		public GameManager(GameContainer container)
		{
			this.container = container;
			SetRange(DateTime.Parse("1900-01-01"), DateTime.Parse("2099-12-31"));
		}

		public void SetRange(DateTime minDate, DateTime maxDate)
		{
			if (maxDate <= minDate)
			{
				throw new ArgumentException("Invalid date range");
			}
			MinDate = minDate.Date;
			MaxDate = maxDate.Date;
			range = (int)maxDate.Subtract(minDate).TotalDays;
		}

		public async Task Reset()
		{
			GameCount = 0;
			TotalGuesses = 0;
			CorrectGuesses = 0;
			await Start();
		}

		public TimeSpan? Elapsed
		{
			get
			{
				if (StartTime == null)
				{
					return null;
				}
				return WinTime.GetValueOrDefault(DateTime.Now).Subtract(StartTime.Value);
			}
		}

		public async Task Start()
		{
			StartTime = null;
			for (int i = 0; i < 10; i++)
			{
				int offset = new Random().Next(range);
				Date = MinDate.AddDays(offset);
				container.Update();
				await Task.Delay(100);
			}
			IsCorrect = null;
			DayGuesses = new bool?[7];
			WinTime = null;
			StartTime = DateTime.Now;
		}

		public bool? IsCorrect { get; internal set; }

		public bool?[] DayGuesses { get; internal set; } = new bool?[7];

		public bool Guess(DayOfWeek day)
		{
			if (!IsCorrect.GetValueOrDefault())
			{
				if (!IsCorrect.HasValue)
				{
					GameCount++;
				}

				TotalGuesses++;
				IsCorrect = Date.DayOfWeek == day;
				if (IsCorrect.Value)
				{
					CorrectGuesses++;
					WinTime = DateTime.Now;
				}
				DayGuesses[(int)day] = IsCorrect;
				//container.Update();
			}
			return IsCorrect.Value;
		}
	}

}
