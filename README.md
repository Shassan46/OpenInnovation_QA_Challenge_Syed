# API Test Automation Project

This project contains automated tests for validating API endpoints. The tests are written in C# using the NUnit framework and leverage RestSharp for HTTP requests. The solution is configured to run on .NET 8.0.

## Table of Contents
- How to Run Tests
- Pipeline Structure
- Advantages and Disadvantages

## How to Run Tests

### Prerequisites
- .NET 8.0 SDK installed on your machine.
- Docker installed and running.
- Docker Compose installed.

### Running Tests Locally

1. Start the API Server:
   - Clone the repository containing the API server code from [this link](https://github.com/openinnovationai/recruiting-qa-challenge.git).
   - Navigate to the project directory and start the server using Docker Compose:
     ```bash
     docker-compose up -d
     ```

2. Run the Tests: 
   - Navigate to the directory containing the `.csproj` file.
   - Restore dependencies, build the solution, and run the tests using the following commands:
     ```bash
     dotnet restore
     dotnet build --configuration Release
     dotnet test --configuration Release
     ```
  Note: You can also use Visual Studio code or Visual Studio 2022 to execute tests

3. View Test Results:
   - Test results are output in the terminal. For more detailed results, you can generate a test report by specifying a logger:
     ```bash
     dotnet test --logger:trx
     ```

### Running Tests via GitHub Actions

Tests are automatically executed when code is pushed to the `master` branch or a pull request is made to `master`. The CI pipeline is defined in `.github/workflows/dotnet-desktop.yml`.

## Pipeline Structure

The CI pipeline is configured using GitHub Actions and includes the following stages:

1. Triggering Events:
   - The pipeline is triggered on push and pull request events to the `master` branch.

2. Checkout Code:
   - The source code is checked out from the repository.

3. Preemptive Disk Cleanup:
   - A cleanup step is performed to ensure sufficient disk space is available.

4. Clone API Server Repository:
   - The pipeline clones the public repository containing the Docker setup for the API server.

5. Install Docker Compose:
   - Docker Compose is installed to orchestrate the multi-container setup.

6. Port Availability Check:
   - The pipeline checks if the required port (8000) is available.

7. Docker Compose Override File Creation:
   - An override file is created to map the correct port for the API server.

8. Build and Start Docker Container:
   - The Docker containers are built and started.

9. Set Up .NET Environment:
   - The .NET 8.0 SDK is set up.

10. Restore, Build, and Test:
    - Dependencies are restored, the project is built, and the tests are executed. The test results are published as a report.

11. Cleanup:
    - The Docker containers and volumes are removed to free up space.

## Advantages and Disadvantages

### Advantages
- Modular and Isolated Testing: The use of NUnit allows for isolated unit tests with easy integration into CI/CD pipelines.
- Automated CI/CD Integration: The GitHub Actions pipeline automates the testing process, ensuring code quality with each commit and pull request.
- Dockerization: The use of Docker ensures that the API server runs in a consistent environment, reducing the likelihood of environment-specific issues.

### Disadvantages
- Complex Setup: The need to configure Docker, Docker Compose, and the API server can be a hurdle, especially for those unfamiliar with containerization.
- Execution Time: The pipeline includes several steps, such as waiting for Docker containers to start, which can increase the overall execution time.
- Dependency on Docker: The entire setup relies on Docker, meaning tests cannot be run in environments where Docker is unavailable or restricted.

---

This README.md should provide sufficient guidance on how to run tests, understand the pipeline, and evaluate the pros and cons of the current approach.
