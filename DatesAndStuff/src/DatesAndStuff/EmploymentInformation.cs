namespace DatesAndStuff
{
    public class EmploymentInformation
    {
        public double Salary { get; private set; }

        public Employer Employer { get; private set; }

        public EmploymentInformation(double salary, Employer e)
        {
            this.Salary = salary;
            this.Employer = e;
        }

        public void IncreaseSalary(double percentage)
        {
            if (percentage <= -10)
            //if (percentage < -10)   // nem kell az az =
                throw new ArgumentOutOfRangeException(nameof(percentage));

            this.Salary = this.Salary * (1 + percentage / 100);
        }
    }
}
