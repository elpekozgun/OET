      subroutine printx(nel,nn,nnmax,nelmax,xcoord,loads,elmat,vvv,
     +  vx,vy,accx,accy,dispx,dispy,strainm,damagenode,
     +  uuu,elnode,damage,strain,forces,nf,nfailed,As,steel,amp)
      implicit none
      
      integer i,j,ii,jj,ikt,nn,nel,m,nnmax,nelmax,nfailed,steel(*)
      integer nf(nnmax,*),elnode(nelmax,*),damage(*),nfiles
     +  
      double precision amp,x1,x2,x3,x4,y1,y2,y3,y4,AA,BB,xx,nlength,
     +  xm,ym,uuu(*),vx(*),vy(*),accx(*),accy(*),dispx(*),dispy(*),
     +  strainm(*),damagenode(*)
      double precision xcoord(nnmax,*),ue(4),loads(*),
     +  elmat(nelmax,*),forces(*),As(*),strain(*),vvv(*)
     
      Character*25 str(15),stri,strf

      nfiles=15

      do i=2,nfiles
      str(i)=''
      enddo

      str(2)='strain'
      str(3)='accx'
      str(4)='accy'
      str(5)='ux'
      str(6)='all'
      str(7)='uy'
      str(8)='vvv'
      str(9)='uuu'
      str(10)='vx'
      str(11)='vy'
      str(12)='strainm'
      str(13)='stp'
      str(14)='damage'
      str(15)='restore'
      
      do i=2,nfiles
      
      j = nfailed
      Write( stri, '(i10)' ) j
      strf=trim(adjustl(str(i)))//trim(adjustl(stri))//'.txt'

      open(10+i,file=strf)

      enddo
    
      do ii=1,nn
      xx=dsqrt(xcoord(ii,1)**2.+xcoord(ii,2)**2.)
        do jj=1,2
             ue(jj)=loads((ii-1)*2+jj)*amp
        enddo
c        write(11,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2)       
        write(18,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),vvv(ii)
        write(19,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),uuu(ii)
        write(20,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),vx(ii)
        write(21,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),vy(ii)    
        write(13,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),accx(ii)
        write(14,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),accy(ii)  
        write(15,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),dispx(ii)
        write(17,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),dispy(ii) 
      write(24,22) xcoord(ii,1)+ue(1),xcoord(ii,2)+ue(2),damagenode(ii)
      enddo       
22    format(4(1pe15.5))

      do i=1,nel
        write(25,22) i,damage(i),strainm(i)
      enddo

      do i=1,nel
      xx=dsqrt(xcoord(elnode(i,1),1)**2.+xcoord(elnode(i,2),2)**2.)
         m=0
         do ii=1,2
         do jj=1,2
            m=m+1
            ue(m)=loads((elnode(i,ii)-1)*2+jj)*amp  
         enddo
         enddo

      if(damage(i).eq.0.and.steel(i).eq.0)then
            x1=xcoord(elnode(i,1),1)+ue(1)
            y1=xcoord(elnode(i,1),2)+ue(2)
            x2=xcoord(elnode(i,2),1)+ue(3)
            y2=xcoord(elnode(i,2),2)+ue(4)
            xm=(x1+x2)/2.
            ym=(y1+y2)/2.
         write(12,18) x1,y1,strain(i)
         write(12,18) x2,y2,strain(i)
         write(12,*) 
      endif

      if(damage(i).eq.1)then
            x1=xcoord(elnode(i,1),1)+ue(1)
            y1=xcoord(elnode(i,1),2)+ue(2)
            x2=xcoord(elnode(i,2),1)+ue(3)
            y2=xcoord(elnode(i,2),2)+ue(4)
            xm=(x1+x2)/2.
            ym=(y1+y2)/2.
         write(22,18) x1,y1,strainm(i)
         write(22,18) x2,y2,strainm(i)
         write(23,18) xm,ym,strainm(i)
         write(22,*) 
      endif
            x1=xcoord(elnode(i,1),1)+ue(1)
            y1=xcoord(elnode(i,1),2)+ue(2)
            x2=xcoord(elnode(i,2),1)+ue(3)
            y2=xcoord(elnode(i,2),2)+ue(4)
            xm=(x1+x2)/2.
            ym=(y1+y2)/2.
         write(16,18) x1,y1,strain(i)
         write(16,18) x2,y2,strain(i)
         write(16,*) 
      
      
c      if(steel(i).eq.1)then
c            x1=xcoord(elnode(i,1),1)+ue(1)
c            y1=xcoord(elnode(i,1),2)+ue(2)
c            x2=xcoord(elnode(i,2),1)+ue(3)
c            y2=xcoord(elnode(i,2),2)+ue(4)
c            xm=(x1+x2)/2.
c            ym=(y1+y2)/2.
c         write(13,18) x1,y1,strain(i)
c         write(13,18) x2,y2,strain(i)
c         write(13,*) 
c      endif
      
c      if(steel(i).eq.1)then
c            x1=xcoord(elnode(i,1),1)+ue(1)
c            y1=xcoord(elnode(i,1),2)+ue(2)
c            x2=xcoord(elnode(i,2),1)+ue(3)
c            y2=xcoord(elnode(i,2),2)+ue(4)
c            xm=(x1+x2)/2.
c            ym=(y1+y2)/2.
c         write(14,18) x1,y1,As(i)
c         write(14,18) x2,y2,As(i)
c         write(14,*)      
c      endif      

  18  format(3e15.5e3)                     
      enddo !elements
    
      do i=2,nfiles
        close(i)
      enddo

      return
      end
      