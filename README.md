# DragonJellyFileUtils 프로젝트 문서

DragonJellyFileUtils는 .NET 응용 프로그램에서 파일 및 드라이브 관련 작업을 손쉽게 처리할 수 있는 유틸리티 라이브러리입니다. 이 라이브러리는 파일 관리, 압축/해제, 드라이브 유효성 검사 등의 기능을 제공하며, 강력한 오류 처리와 비동기 작업 지원, 진행률 추적 기능을 포함하고 있습니다.

---

## 주요 기능

- **파일 관리(FileManager)**
  - 파일 복사, 이동, 생성 및 보안 삭제 지원.
  - 비동기 작업을 통한 효율적인 처리.
- **파일 압축 및 해제(FileCompression)**
  - 디렉터리 및 파일 압축/해제 기능 제공.
  - 사용자 지정 압축 수준과 진행률 추적 지원.
- **드라이브 유틸리티(DriveUtils)**
  - 드라이브 공간 유효성 검사 및 안전한 파일 전송 보장.

---

## 설치 방법

이 저장소를 클론하고 프로젝트에 추가합니다:

```bash
git clone https://github.com/LeeHongGu/DragonJellyFileUtils.git
```

응용 프로그램에서 `DragonJellyFileUtils` 프로젝트를 참조로 추가하세요.

---

## 사용 예제

### 1. 파일 관리(FileManager)

#### 파일 복사
```csharp
await FileManager.CopyFileAsync("source.txt", "destination.txt", overwrite: true);
```

#### 파일 이동
```csharp
await FileManager.MoveFileAsync("source.txt", "destination.txt");
```

#### 빈 파일 생성
```csharp
await FileManager.CreateEmptyFileAsync("newfile.txt");
```

#### 파일 보안 삭제
```csharp
await FileManager.SecureDeleteFileAsync("sensitive-data.txt");
```

---

### 2. 파일 압축 및 해제(FileCompression)

#### 파일 또는 디렉터리 압축
```csharp
var progress = new Progress<ProgressReport>(report =>
{
    Console.WriteLine($"Progress: {report.BytesProcessed}/{report.TotalBytes}");
});

await FileCompression.CompressAsync("source", "destination.zip", CustomCompessionLevel.Optimal, progress);
```

#### 파일 압축 해제
```csharp
await FileCompression.DecompressAsync("source.zip", "destination");
```

#### 사용자 지정 압축 수준
```csharp
await FileCompression.CompressAsync("source", "destination.zip", CustomCompessionLevel.Fastest);
```

| 압축 수준               | 설명                                   |
|-------------------------|--------------------------------------|
| `Fastest`              | 속도를 우선시하며 압축률이 낮음.       |
| `Optimal`              | 속도와 압축률 간 균형 유지.           |
| `NoCompression`        | 압축하지 않고 원본 파일로 아카이브 생성. |

---

### 3. 드라이브 유틸리티(DriveUtils)

#### 파일 또는 디렉터리가 이동 가능한지 확인
```csharp
bool canMove = DriveUtils.CanMoveItem("source", "D:\\");
if (canMove)
{
    Console.WriteLine("파일을 이동할 수 있습니다.");
}
else
{
    Console.WriteLine("대상 드라이브에 충분한 공간이 없습니다.");
}
```

#### 드라이브 유효성 검사 예제
```csharp
try
{
    bool canMove = DriveUtils.CanMoveItem("source", "E:\\");
    Console.WriteLine(canMove ? "이동 가능!" : "이동 불가!");
}
catch (Exception ex)
{
    Console.WriteLine($"오류: {ex.Message}");
}
```

---

## 메서드 개요

### FileManager
| 메서드                    | 설명                                                                       |
|--------------------------|--------------------------------------------------------------------------|
| `CopyFileAsync`          | 파일을 비동기로 복사. 덮어쓰기 및 진행률 추적 지원.                           |
| `MoveFileAsync`          | 파일을 복사하고 원본을 보안 삭제하여 이동.                                     |
| `CreateEmptyFileAsync`   | 새로운 빈 파일 생성.                                                       |
| `SecureDeleteFileAsync`  | 랜덤 바이트로 덮어쓰고 파일 삭제.                                           |

### FileCompression
| 메서드                    | 설명                                                                       |
|--------------------------|--------------------------------------------------------------------------|
| `CompressAsync`          | 파일 또는 디렉터리를 압축. 진행률 추적 지원.                                 |
| `DecompressAsync`        | ZIP 아카이브에서 파일 추출.                                                |

### DriveUtils
| 메서드                    | 설명                                                                       |
|--------------------------|--------------------------------------------------------------------------|
| `CanMoveItem`            | 드라이브 공간을 확인하여 파일 또는 디렉터리 이동 가능 여부를 평가.                |

---

## 기여하기

이 프로젝트에 기여하고 싶다면 언제든지 환영합니다! 새로운 기능을 제안하거나 버그를 보고하려면 풀 리퀘스트를 제출하거나 이슈를 열어주세요.

---

## 문의

질문이나 지원 요청은 [dlghdrn312@gmail.com](mailto:dlghdrn312@gmail.com)으로 연락 주세요.

---

