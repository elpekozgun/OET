      subroutine energy2(nn,nnmax,nel,nelmax,xcoord,elnode,
     +    elmat,energyn,ycoord)
      implicit none
      integer nel,nnmax,nelmax,nn
      integer elnode(nelmax,*)
      double precision elmat(nelmax,*),xcoord(nnmax,*),ycoord(nnmax,*)
      double precision KE(4,4),T(4,4),KET(4,4),cbeta,sbeta
      double precision ue(4),fe(4),energyn(*),
     +  xx
      integer i,ii,jj,m,j,k,idir

        
      do i=1,2
        energyn(i)=0.
      enddo

      do idir=1,2
      
      if(idir.eq.1)then
      do i=1,nn
        ycoord(i,1)=xcoord(i,1)*1.001      
        ycoord(i,2)=xcoord(i,2)
      enddo
      else
      do i=1,nn
        ycoord(i,2)=xcoord(i,2)*1.001      
        ycoord(i,1)=xcoord(i,1)
      enddo        
      endif
 
      do j=1,nel
        call formd6(elmat(j,1),elmat(j,2),elmat(j,3),KE)
        cbeta=xcoord(elnode(j,2),1)-xcoord(elnode(j,1),1)
        sbeta=xcoord(elnode(j,2),2)-xcoord(elnode(j,1),2)
        cbeta=cbeta/elmat(j,3)
        sbeta=sbeta/elmat(j,3)
        call formT6(cbeta,sbeta,T)
        call matmul1(KE,4,T,4,KET,4,4,4,4)
        ue(1)=ycoord(elnode(j,1),1)-xcoord(elnode(j,1),1)
        ue(2)=ycoord(elnode(j,1),2)-xcoord(elnode(j,1),2)
        ue(3)=ycoord(elnode(j,2),1)-xcoord(elnode(j,2),1)
        ue(4)=ycoord(elnode(j,2),2)-xcoord(elnode(j,2),2)

        do ii=1,4
          xx=0.
          do jj=1,4
            xx=xx+KET(ii,jj)*ue(jj)
          enddo
          fe(ii)=xx
       enddo
               energyn(idir)=energyn(idir)+
     +     fe(1)**2.*elmat(j,3)/(2.*elmat(j,1))

        enddo

      enddo

      do j=1,2
            write(*,*) 'direction and energy ',j,energyn(j)
      enddo        

      return
      end
      
      subroutine MATMUL1(A,IA,B,IB,C,IC,L,M,N)
C
C      FORMS THE PRODUCT OF TWO MATRICES
C
      IMPLICIT NONE
      INTEGER IA,IB,IC
      DOUBLE PRECISION  A(IA,*),B(IB,*),C(IC,*)
      INTEGER L,M,N,I,J,K
      DOUBLE PRECISION X

      DO 1 I=1,L
      DO 1 J=1,N
      X=0.0
      DO 2 K=1,M
    2 X=X+A(I,K)*B(K,J)
      C(I,J)=X
    1 CONTINUE
      RETURN
      END
      
      subroutine formd6(EA,EI,L,KE)
      implicit none
      integer i,j
      double precision KE(4,4),EA,EI,L
	
      KE(1,1)=EA/L
      KE(1,2)=0.
      KE(1,3)=-EA/L
      KE(1,4)=0.
      KE(2,1)=0.
      KE(2,2)=0.
      KE(2,3)=0.
      KE(2,4)=0.
      KE(3,1)=-EA/L
      KE(3,2)=0.
      KE(3,3)=EA/L
      KE(3,4)=0.
      KE(4,1)=0.
      KE(4,2)=0.
      KE(4,3)=0.
      KE(4,4)=0.

      return
      end

      subroutine formT6(cbeta,sbeta,T)
      implicit none
      double precision cbeta,sbeta,T(4,4)
	
      call null(T,4,4,4)

      T(1,1)=cbeta
      T(1,2)=sbeta
      T(2,1)=-sbeta
      T(2,2)=cbeta
      T(3,3)=cbeta
      T(3,4)=sbeta
      T(4,3)=-sbeta
      T(4,4)=cbeta

      return
      end
      
      subroutine formTT6(cbeta,sbeta,TT)
      implicit none
      double precision cbeta,sbeta,TT(4,4)
	
      call null(TT,4,4,4)

      TT(1,1)=cbeta
      TT(1,2)=-sbeta
      TT(2,1)=sbeta
      TT(2,2)=cbeta
      TT(3,3)=cbeta
      TT(3,4)=-sbeta
      TT(4,3)=sbeta
      TT(4,4)=cbeta

      return
      end
      
      subroutine NULL(A,IA,M,N)
C
C      NULLS A 2-D ARRAY
C
      IMPLICIT NONE
      INTEGER IA
      DOUBLE PRECISION A(IA,*)
      INTEGER M,N,I,J


      DO 1 I=1,M
      DO 1 J=1,N
    1 A(I,J)=0.0
      RETURN
      END
 
      subroutine FORMCOL2D(ikb,nnmax,nelmax,ROW,COL,nelem,
     +    NF,NN,NEL,N)
C
C     STRUCTURE OF STIFFNESS MATRIX
C
      IMPLICIT NONE
      INTEGER ikb,nnmax,nelmax
      INTEGER ROW(*),COL(*),NELEM(nelmax,*),NF(nnmax,*)
      INTEGER NEL,N,NN
      INTEGER I,J,K,II,JJ,III,NUN,NBEFORE,NONZ,IP
      integer ikt,jkt

      DO 101 I=1,IKB
 101  COL(I)=0
      DO 102 I=1,N+1
 102  ROW(I)=0
C
      ROW(1)=1 
      NONZ=0
      NUN=0

      do 1 I=1,nn
      if(mod(i,nn/10).eq.0)write(*,*) 'formcol ',(i*1./nn)*100,'%',
     +  nonz,ikb

      DO 1 J=1,2

      IF(NF(I,J).NE.0)THEN
      NUN=NUN+1
      NBEFORE=NONZ+1
      do 10 ip=1,nel
         DO 10 K=1,2
            IF(NELEM(ip,K).EQ.I)THEN
               DO 20 II=1,2
               DO 20 JJ=1,2
                  IF(NF(NELEM(ip,II),JJ).NE.0)THEN
                     DO 30 III=NBEFORE,NONZ+1
                        IF(COL(III).EQ.NF(NELEM(ip,II),JJ))THEN
                        GOTO 20
                        ENDIF
 30                  CONTINUE
                     NONZ=NONZ+1
                     if(nonz.eq.ikb)then
                      write(*,*) 'increase ikb '
                      stop
                     endif
                     ROW(NUN+1)=NONZ+1
                     COL(NONZ)=NF(NELEM(ip,II),JJ)
                  ENDIF
 20            CONTINUE
            ENDIF


 10      CONTINUE
      ENDIF
 1    CONTINUE
       
c      write(*,*) 'number of nonzero terms ',nonz
      RETURN
      END
      
      subroutine MATRAN(A,IA,B,IB,M,N)
C
C      FORMS THE TRANSPOSE OF A MATRIX
C
      IMPLICIT NONE
      INTEGER IA,IB
      DOUBLE PRECISION A(IA,*),B(IB,*)
      INTEGER M,N,I,J

      DO 1 I=1,M
      DO 1 J=1,N
    1 A(J,I)=B(I,J)
      RETURN
      END

      subroutine FORMK2D(KB,KM,ikm,ROW,COL,G,N)
C
C     FORMS KB AS A COMPRESSED VECTOR
C
      IMPLICIT NONE

      integer ikm
      DOUBLE PRECISION KB(*),KM(ikm,*)
      INTEGER G(*),ROW(*),COL(*)
      INTEGER N
      INTEGER I,J,KK

      DO 11 I=1,4
         IF(G(I))11,11,22
 22      DO 33 J=1,4
            IF(G(J))33,33,44
            
 44         DO 1 KK=ROW(G(I)),ROW(G(I)+1)-1,1
               IF(COL(KK).EQ.G(J))THEN
                  KB(KK)=KB(KK)+KM(I,J)
                  GOTO 33
               ENDIF
 1          CONTINUE
            
 33      CONTINUE
 11   CONTINUE
      
 241  continue
      RETURN
      END

      subroutine CG(A,X,R,D,H,N,ROW,COL,TOL,dia,iflag,nn)
C
C     CONJUGATE GRADIENT
C
      IMPLICIT NONE
      DOUBLE PRECISION  A(*),X(*),R(*),D(*)
      double precision H(*),dia(*)
      double precision xxxx
      INTEGER ROW(*),COL(*)
      INTEGER N,iflag
      DOUBLE PRECISION TOL,XX,DEL0,DEL1,BETA,TAU,aa
      INTEGER I,J,NN,jj
      
      do i=1,n
        r(i)=0.
        d(i)=0.
        h(i)=0.
        dia(i)=0.
      enddo

      iflag=0
      xx=0.
      del0=0.
      del1=0.
      beta=0.
      tau=0.
      xxxx=0.
c
c     obtain the diagonal vector dia
c
      do i=1,n
      do j=row(i),row(i+1)-1
      if(col(j).eq.i)then
      dia(i)=a(j)
      endif
      enddo
      enddo

c
c     now precon-cg
c
      NN=0

      xx=0.
      do i=1,n
          r(i)=-x(i)
          x(i)=0.
          h(i)=r(i)/dia(i)
          d(i)=-h(i)
          xx=xx+r(i)*h(i)
      enddo
  
      DEL0=XX

      IF(DEL0.LT.TOL)GOTO 99

 4    continue
 
      xxxx=0
      DO I=1,n
          XX=0.
          DO J=ROW(I),ROW(I+1)-1
              XX=XX+A(J)*D(COL(J))
          enddo
          H(I)=XX
          xxxx=xxxx+d(i)*h(i)
      enddo

      xx=xxxx
      
      if(del0.lt.tol.and.xx.lt.tol)then
!        write(*,*) 'cg warning ',nn,del0,xx
!        pause
!        goto 99
      endif
        
      TAU=DEL0/XX

      xx=0.
      DO I=1,n
          X(I)=X(I)+TAU*D(I)
          R(I)=R(I)+TAU*H(I)
          h(i)=r(i)/dia(i)
          xx=xx+r(i)*h(i)
      enddo 

      NN=NN+1
      DEL1=XX

      xx=-1.
      do i=1,n
         if(dabs(tau*d(i)).ge.xx)then
            xx=dabs(tau*d(i))
         endif
      enddo

c      write(*,*) nn,xx
c      if(mod(nn,100).eq.0)write(*,*) nn,xx,x(5000)

      if(xx.lt.tol)goto 99

      IF(NN.GT.N*2)then
         write(*,*) 'cg did not converge in ',n*2,' iterations ',xx
         write(88,*) 'cg did not converge in ',n*2,' iterations ',xx
         iflag=0
         goto 99
      endif

      BETA=DEL1/DEL0
      DEL0=DEL1

      DO 10 I=1,N
         D(I)=-H(I)+BETA*D(I)
 10   CONTINUE

      GOTO 4

 99   CONTINUE

      iflag=1
c      WRITE(*,*) '#CG IT=',NN,xx,' unknowns = ',n

      RETURN
      END

      subroutine normald(iseed,n,array,mean,sd)
      implicit none
      integer n,iseed,i
      double precision array(*),mean,sd,temp,pi,drand,mean2,sd2
            
      call srand(iseed)

      pi = 4.0*DATAN(1.0d0)

      do i=1,n
        array(i)=drand(0)
      enddo

 !  Now convert to normal distribution
      DO i = 1, n-1, 2
      temp = sd * SQRT(-2.0*LOG(array(i))) * COS(2*pi*array(i+1)) + mean
!      write(*,*) temp
      array(i+1) = sd * SQRT(-2.0*LOG(array(i))) * SIN(2*pi*array(i+1))
     +   + mean
      array(i) = temp
!      write(*,*) array(i)
      END DO

 
! Check mean and standard deviation
      mean2=0.
      sd2=0.
      do i=1,n
!      write(*,*) i,array(i)
      mean2=mean2+array(i)
      enddo
      mean2=mean2/n

      do i=1,n
      sd2=sd2+((array(i)-mean2)**2)
      enddo
      
      sd2 = SQRT(sd2/n)
 
      WRITE(*,*) mean2,sd2
      
      return
      end