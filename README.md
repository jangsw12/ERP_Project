# ERP_Project

## **🗂️ 프로젝트 개요**

- **개발 기간 :** 2026.02 ~ 2026.03 (약 1개월)
- **개발 환경 :**
    - Visual Studio 2022
    - .NET 8.0 (WPF)
    - SQL Server Management Studio 20
- **사용 기술 :** C# / XAML / MS SQL Server
- **개발 형태 :** 개인 프로젝트
- **아키텍처 및 디자인 패턴 :** MVVM / Service Pattern / Dependency Injection

---

## 🎯 **프로젝트 소개**

- 제조업 ERP 시스템을 가정하여 **구매, 생산, 판매 프로세스를 데이터베이스 중심으로 설계한 개인 프로젝트**입니다.
- ERP의 핵심 업무 흐름을 반영하여 데이터 구조를 설계하고, 재고를 **Transaction 기반으로 관리**하도록 구현했습니다.
- UI는 WPF 기반으로 간단한 조회 및 관리 화면으로 구성했으며, 데이터 조회는 **Stored Procedure 기반으로 처리**했습니다.

---

## **🧮 ERD 구조**

🔹**주요 테이블 구성**

- **기본 마스터 데이터**
    
    AppUser			- 사용자 테이블
  
    Item				- 품목 테이블
  
    Customer			- 거래처 테이블
  
    Warehouse			- 창고 테이블
    
- **구매 도메인**
    
    PurchaseOrderM	- 발주 마스터 테이블
  
    PurchaseOrderD		- 발주 디테일 테이블
    
- **생산 도메인**
    
    ProductionOrderM	- 생산 마스터 테이블
  
    ProductionOrderD	- 생산 디테일 테이블
    
- **판매 도메인**
    
    SalesOrderM		- 수주 마스터 테이블
  
    SalesOrderD		- 수주 디테일 테이블
    
- **재고 및 생산 관리**
    
    BOM				- BOM 테이블
  
    Inventory			- 재고 테이블
  
![image.png](attachment:3b6d168a-66df-424a-a93f-c4e512d1b4ed:image.png)
