      implicit none
      integer nnmax,nelmax,iloads,maxncracks,maxntimedata,
     +  maxnmaterials,maxnmonitor  
      parameter (nnmax=800000,iloads=2*nnmax)
      parameter (nelmax=2000000,maxncracks=10)
      parameter (maxntimedata=50000,maxnmaterials=10)         
      parameter (maxnmonitor=10)

      double precision xcoord(nnmax,2),As(nelmax),energyn(2),
     +  ycoord(nnmax,2),elmat(nelmax,3),Area,ff,EA,w,delta,
     +  loads(iloads),u(iloads),rr(2*maxnmonitor),
     +  uu,ey,eult,sult,amp,tt,mm(iloads),
     +  dx,gravity,damagedof(iloads),Es,gravityo,
     +  uprint(iloads),strain(nelmax),forces(nelmax),maxu,
     +  mass,vvv(nnmax),uuu(nnmax),vx(nnmax),vy(nnmax),
     +  v(iloads),dt,alpha,accx(nnmax),accy(nnmax),sum,
     +  maxv,maxa,vv,strainm(nelmax),dispx(nnmax),dispy(nnmax),
     +  ttmax,xx,xx1,xx2,tthold,
     +  damagenode(nnmax),damping,beta,loadsold(iloads),
     +  betaint,beta1,beta2,gamma1,gamma2,accmult,
     +  xlevel1(maxncracks),ylevel1(maxncracks),xlevel2(maxncracks),
     +  ylevel2(maxncracks),crackstrain(maxncracks),
     +  xxi,xxj,yyi,yyj,xcr1,xcr2,f1,f2,t1,t2,f,
     +  xcr3,xcr4,drand,Ec(maxnmaterials),density(maxnmaterials),
     +  a1(maxnmaterials),a2(maxnmaterials),a3(maxnmaterials),
     +  b1(maxnmaterials),b2(maxnmaterials),epscr(maxnmaterials),
     +  dsize(maxnmaterials),loadingvalue(10,10,2),trif(10,4),
     +  triff(10,1000),tphase1,gravityacclimit,averagea,Rxn(2),
     +  vdesired,lambda,rhs,fold,rhsold,frate,vboundary,dtfactor

      integer ntimedata,phase
      double precision tim(maxntimedata),uux(maxntimedata),
     +  uuy(maxntimedata),ux,uy

      integer elnode(nelmax,2),steel(nelmax),nmonitor,nmonitor2,
     +  nf(nnmax,3),damage(nelmax),nfailed,monitor(maxnmonitor,3),
     +  monitor2(maxnmonitor,4),tri(10,2),nntri(10,1000),ntri(10),
     +  pfreq,pfreq2,pfreq3,nfailedold,steeln(nnmax),ii,
     +  eltype(nelmax),nmaterials,ncracks,nvcont1,nvcont2,
     +  nodeelement(nnmax,100),readacc,nbase,base(maxnmonitor,2),
     +  nfm(nnmax,2),vboundarydirection
     
      integer nn,nel,steelflag,readdamage,i,j,m,n,ikt,k,iii,
     +  i1,i2,i3,i4,i5,i6,externalloadtype,nloads,loading(10,10,4),
     +  loadinglines(10),loadingvaluelines(10),igravity,
     +  loadingtype(10)
      Character*35 file1,file2,file3,file4,file5
      Character*25 stri,strf,ddd,accfile
           
      write(*,*) 'nnmax nelmax iloads '
      write(*,*) nnmax,nelmax,iloads
c
c     TO DO LIST
c     - added mass 
c     - mass / different materials (steel/concrete/others)
c
c     1 < beta < 28/27  -> integration parameter
      betaint=1.01
         
      write(*,*) 'input file ?'
      read(*,*) stri
      write(*,*) 'reinforcement 0 / 1'
       
      file1=trim(adjustl(stri))//'.control.inp'
      file2=trim(adjustl(stri))//'.mesh.inp'
      file3=trim(adjustl(stri))//'.boundary.inp'    
      file4=trim(adjustl(stri))//'.loading.inp'  
      file5=trim(adjustl(stri))//'.output.inp' 
      
      write(*,*) file1
      write(*,*) file2
      write(*,*) file3

      open(11,file=file1)
      open(12,file=file2)

      read(11,*) dt,w,alpha,beta
      read(11,*) ttmax,amp,pfreq,pfreq2,pfreq3
      read(11,*) nmaterials
      
      if(nmaterials.gt.maxnmaterials)then
        write(*,*) 'incrase maxnmaterials '
        stop
      endif
      
      do i=1,nmaterials
        read(11,*) Ec(i),density(i),epscr(i),
     +  a1(i),a2(i),a3(i),b1(i),b2(i),dsize(i)
      enddo
      
      read(11,*) steelflag,ey,eult,sult  
      
      read(11,*) readacc
      if(readacc.eq.1)then
      read(11,*) accfile,accmult
      endif
      
      read(11,*) ncracks
      if(ncracks.gt.maxncracks)then
        write(*,*) 'increase maxncracks'
        stop
      endif
      
      do i=1,ncracks
      read(11,*) xlevel1(i),ylevel1(i),xlevel2(i),
     +  ylevel2(i),crackstrain(i)
      enddo
      
      read(11,*) readdamage
      if(readdamage.eq.1)then
        read(11,*) ddd
      endif

      write(*,*) 'completed reading control.inp'
      close(11) 
            
      pause
        
      read(12,*) nn,nel,area,dx
      
      if(nn.gt.nnmax)then
        write(*,*) 'increase nnmax',nn,nnmax
        stop
      endif
      
      if(nel.gt.nelmax)then
        write(*,*) 'increase nelmax',nel,nelmax
        stop
      endif
      
      do i=1,nn
        read(12,*) j,xcoord(i,1),xcoord(i,2),steeln(i)
        steeln(i)=steeln(i)*steelflag
      enddo
     
      do i=1,nel
        read(12,*) j,elnode(i,1),elnode(i,2),steel(i),As(i),eltype(i)
        steel(i)=steel(i)*steelflag
        As(i)=As(i)*steelflag
        damage(i)=0
        strainm(i)=0.
      enddo
      close(12)
      write(*,*) 'completed reading mesh.inp'
      write(*,*) 'number of nodes ',nn
      write(*,*) 'number of elements ',nel
      write(*,*) 'dx ',dx
      write(*,*) 'area ',area
      pause

      if(readdamage.eq.1)then
          open(12,file=ddd)
          write(*,*) 'reading damage data from ',ddd
          do i=1,nel
            read(12,*) j,damage(i),strainm(i)
          enddo
          close(12)
      endif
      
      do i=1,nel
        elmat(i,1)=1.
        elmat(i,2)=0.
        elmat(i,3)=
     +      dsqrt((xcoord(elnode(i,1),1)-xcoord(elnode(i,2),1))**2.+
     +            (xcoord(elnode(i,1),2)-xcoord(elnode(i,2),2))**2.)
      enddo

      call energy2(nn,nnmax,nel,nelmax,xcoord,elnode,
     +    elmat,energyn,ycoord)
c     plane stress     
      ff=0.001*0.001*0.5625*Ec(1)*(w*Area)
c     plane strain
c      ff=0.001*0.001*0.75*Ec*w*Area
	      
	EA=ff/(0.5*energyn(1)+0.5*energyn(2))
c     ?????
c	EA=ff/energyn(1)
      write(*,*) 'EA ',EA

      pause      

      m=0.
      do i=1,nel
        if(steel(i).eq.0)then
          elmat(i,1)=elmat(i,1)*EA
          elmat(i,1)=elmat(i,1)*Ec(eltype(i))/Ec(1)
        else
          elmat(i,1)=As(i)*Es
          m=m+1
        endif      
      enddo
      write(*,*) 'number of steel elements ',m

      do i=1,nn
        do j=1,2
            nf(i,j)=1
            nfm(i,j)=0
        enddo
      enddo  
c
c     boundary conditions
c
      open(13,file=file3)

      i6=1
      write(*,*) 'supports '
      write(*,*) 'node, x, y, coordinate'
      do while (i6.eq.1)
         read(13,*) i1,i2,i3,i4,i5,i6
         do j=i1,i2,i3
         write(*,123) j,i4,i5,xcoord(j,1),xcoord(j,2)
123   format(3i5,2(1pe15.5))
            nf(j,1)=i4
            nf(j,2)=i5
         enddo
      enddo

      read(13,*) i1

      if(i1.eq.1)then
      read(13,*) vboundary,vboundarydirection
      write(*,*) 'boundary velocity ',vboundary,
     +  'boundary direction ',vboundarydirection
      i6=1
      write(*,*) 'specified velocities' 
      do while (i6.eq.1)
         read(13,*) i1,i2,i3,i6
         do j=i1,i2,i3
            nfm(j,vboundarydirection)=1
            write(*,124) j,vboundarydirection
 124        format(3i6)
         enddo
      enddo 
      endif            
      
      close(13) 
      write(*,*) 'completed reading ',file3
      
      n=0
      do i=1,nn
        do j=1,2
            if(nf(i,j).eq.1)then
                n=n+1
                nf(i,j)=n
            endif
        enddo
      enddo      
      write(*,*) 'nodes',nn,' elements=',nel,'unknowns=',n        
      if(iloads.lt.n)then
        write(*,*) 'increase iloads ',iloads,n
        stop
      endif
      pause
c
c     output
c      
      open(11,file=file5)
      read(11,*) nmonitor
      if(nmonitor.gt.maxnmonitor)then
        write(*,*) 'increase maxnmonitor '
        stop
      endif
      do i=1,nmonitor
        read(11,*) monitor(i,1),monitor(i,2),monitor(i,3)
      enddo 
      
      read(11,*) nmonitor2
      if(nmonitor2.gt.maxnmonitor)then
        write(*,*) 'increase the size of monitor2'
        stop
      endif
      do i=1,nmonitor2
        read(11,*) monitor2(i,1),monitor2(i,2),
     +  monitor2(i,3),monitor2(i,4)
      enddo
      
      read(11,*) nbase
      if(nbase.gt.maxnmonitor)then
        write(*,*) 'increase the size of base'
        stop
      endif
      do i=1,nbase
        read(11,*) base(i,1),base(i,2)
      enddo
      
      do i=1,nmonitor
        j=i
        write( stri, '(i10)' ) j
        strf='m'//trim(adjustl(stri))//'.txt'
        open(30+i,file=strf)
        write(*,*) 'monitor file ',strf
        write(*,*) 'unit ',30+i,monitor(i,1),
     +  monitor(i,2),monitor(i,3)
      enddo      
      
      do i=1,nmonitor2
        j=i+nmonitor
        write( stri, '(i10)' ) j
        strf='m'//trim(adjustl(stri))//'.txt'
        open(30+j,file=strf)
        write(*,*) 'relative monitor file ',strf
        write(*,*) 'unit ',30+j,monitor2(i,1),
     +  monitor2(i,2),monitor2(i,3),monitor2(i,4)
      enddo 
      
      do i=1,nbase
        j=i
        write( stri, '(i10)' ) j
        strf='B'//trim(adjustl(stri))//'.txt'
        open(30+i+nmonitor+nmonitor2,file=strf)
        write(*,*) 'Support Force diagrams ',strf
        write(*,*) base(i,1),base(i,2)        
      enddo
      open(666,file='rxn1.txt')
      open(667,file='rxn2.txt')
                
      close(11)
      write(*,*) 'completed reading ',file5
      pause
c
c     loads
c      
      open(13,file=file4)      
      read(13,*) externalloadtype,nvcont1,nvcont2,vdesired,lambda
      read(13,*) igravity
      read(13,*) gravity,tphase1,gravityacclimit
      gravityo=gravity
      if(externalloadtype.eq.1.or.externalloadtype.eq.2)then

          read(13,*) nloads

          do i=1,nloads
          if(nloads.gt.10)then
            write(*,*) 'increase array sizes for loading related arrays'
            stop
          endif
                
          read(13,*) loadingtype(i)

          if(loadingtype(i).eq.1)then
              i5=1
              k=0
              do while (i5.eq.1)
                k=k+1
                read(13,*) i1,i2,i3,i4,i5
                loading(i,k,1)=i1
                loading(i,k,2)=i2
                loading(i,k,3)=i3
                loading(i,k,4)=i4
              enddo
              loadinglines(i)=k
              i5=1
              k=0
              do while (i5.eq.1)
                k=k+1
                read(13,*) loadingvalue(i,k,1),loadingvalue(i,k,2),i5
              enddo
              loadingvaluelines(i)=k
          endif
          if(loadingtype(i).eq.2)then
              i5=1
              k=0
              do while (i5.eq.1)
                k=k+1
                read(13,*) i1,i2,i3,i5
                loading(i,k,1)=i1
                loading(i,k,2)=i2
                loading(i,k,3)=i3
              enddo
              loadinglines(i)=k        
              read(13,*) tri(i,1),trif(i,1),tri(i,2),trif(i,2),
     +              trif(i,3),trif(i,4)
     
              xx=dsqrt((xcoord(tri(i,1),1)-xcoord(tri(i,2),1))**2.+
     +                 (xcoord(tri(i,1),2)-xcoord(tri(i,2),2))**2.)

              i1=0
              do k=1,loadinglines(i)  
                do j=loading(i,k,1),loading(i,k,2),loading(i,k,3)
              i1=i1+1
              if(i1.gt.1000)then
                write(*,*) 'increase array size for triangular loadings'
                stop
              endif
              xx1=dsqrt((xcoord(tri(i,1),1)-xcoord(j,1))**2.+
     +                  (xcoord(tri(i,1),2)-xcoord(j,2))**2.)
              xx2=dsqrt((xcoord(j,1)-xcoord(tri(i,2),1))**2.+
     +                  (xcoord(j,2)-xcoord(tri(i,2),2))**2.)     
              nntri(i,i1)=j
              triff(i,i1)=trif(i,1)*xx2/xx+trif(i,2)*xx1/xx
                enddo
              enddo
              ntri(i)=i1
                        
              i5=1
              k=0
              do while (i5.eq.1)
                k=k+1
                read(13,*) loadingvalue(i,k,1),loadingvalue(i,k,2),i5
              enddo
              loadingvaluelines(i)=k
          
          endif            
          enddo

          write(*,*) 
          write(*,*) 'applied loads '
          do i=1,nloads
            if(loadingtype(i).eq.1)then
            write(*,*) 'loading type uniform',loadingtype(i)
            do k=1,loadinglines(i)  
                do j=loading(i,k,1),loading(i,k,2),loading(i,k,3)
                    write(*,*) 'node ',j,' direction ',loading(i,k,4)
                enddo
            enddo
            do k=1,loadingvaluelines(i)
                write(*,*) loadingvalue(i,k,1),loadingvalue(i,k,2)
            enddo
            endif
            if(loadingtype(i).eq.2)then
            write(*,*) 'loading type linear',loadingtype(i)
            write(*,*) 'node factor cos theta cos beta '
            do j=1,ntri(i)
                write(*,*) nntri(i,j),triff(i,j),trif(i,3),tri(i,4)
            enddo
            
            do k=1,loadingvaluelines(i)
                write(*,*) loadingvalue(i,k,1),loadingvalue(i,k,2)
            enddo          
            endif

          enddo

      endif
      
      write(*,*) 'completed reading ',file4
      close(13)

      
      do i=1,nn
        do j=1,2
            ycoord(i,j)=xcoord(i,j)
        enddo
      enddo
c
c     fix density !
c
      mass=dx*dx*w*density(1)
      do i=1,n
        mm(i)=mass
      enddo
c
c     ?????
c
      do i=1,nel
        yyi=xcoord(elnode(i,1),2)
        yyj=xcoord(elnode(i,2),2)    
        if(0.5*(yyi+yyj).lt.0.15)then
        if(0.5*(yyi+yyj).lt.0.0)then
            elmat(i,1)=2.*elmat(i,1)
        else
            elmat(i,1)=elmat(i,1)*(2.0-1.0*0.5*(yyi+yyj)/0.15)         
        endif
       endif
      enddo
     
      if(readacc.eq.1)then 
      
      open(13,file=accfile)   
      write(*,*) accfile 
          
      do i=1,maxntimedata
         if(i.gt.maxntimedata)then
            write(*,*) 'increase maxntimedata'
            stop
         endif
         read(13,*,end=24) tim(i),uux(i),uuy(i)
      enddo
 24   continue
      close(13)

      ntimedata=i-1
      write(*,*) 'acceleration data size',ntimedata,tim(ntimedata)
      write(*,*) 'accmultiplier ',accmult
      endif
      
      do i=1,nn
      k=0
      do j=1,nel
        if(elnode(j,1).eq.i.or.elnode(j,2).eq.i)then
            k=k+1
            nodeelement(i,k+1)=j
        endif
      enddo
      nodeelement(i,1)=k
      if(k.eq.0)then
        write(*,*) 'unconnected node'
        pause
      endif
      enddo 

      do k=1,ncracks
            
c      ylevel1=drand(0)
c      ylevel2=drand(0)
c      xlevel1=drand(0)
c      xlevel2=drand(0)

      do i=1,nel
        xxi=xcoord(elnode(i,1),1)
        yyi=xcoord(elnode(i,1),2)
        xxj=xcoord(elnode(i,2),1)
        yyj=xcoord(elnode(i,2),2)      
        xcr1=(xlevel2(k)-xlevel1(k))*(yyi-ylevel1(k))
     +      -(ylevel2(k)-ylevel1(k))*(xxi-xlevel1(k))    
        xcr2=(xlevel2(k)-xlevel1(k))*(yyj-ylevel1(k))
     +      -(ylevel2(k)-ylevel1(k))*(xxj-xlevel1(k))        
        xcr3=(xxj-xxi)*(ylevel1(k)-yyi)
     +      -(yyj-yyi)*(xlevel1(k)-xxi)    
        xcr4=(xxj-xxi)*(ylevel2(k)-yyi)
     +      -(yyj-yyi)*(xlevel2(k)-xxi)  

        if((xcr1*xcr2).lt.0.0d00.and.(xcr3*xcr4).lt.0.0d0)then
           damage(i)=1
           strainm(i)=crackstrain(k)
        endif 
      
      enddo    
      enddo
      pause

      do i=1,n
        u(i)=0.
        v(i)=0.
        loads(i)=0.
        loadsold(i)=0.
      enddo

      tt=0.
      tthold=0.0
      ux=0.
      uy=0.
      phase=1
      if(igravity.eq.0)phase=2
      
      ikt=0
      nfailedold=0
      rhsold=0.0
      fold=0.0
      f=0.0
      frate=0.0
      do i=1,nmonitor+nmonitor2
        rr(i)=0.0
      enddo
      
      beta1=dt*dt*(0.5d0-betaint)
      beta2=dt*dt*betaint
      gamma1=-0.5d0*dt
      gamma2=1.5d0*dt
      write(*,*) 'phase ',phase
      
241   continue

      ikt=ikt+1        
      tt=tt+dt 

      if(phase.eq.2)then
  
          ii=0
          do i=1,ntimedata-1
            if(tt.ge.tim(i))then
            if(tt.le.tim(i+1))then
               ux=uux(i)+(uux(i+1)-uux(i))/(tim(i+1)-tim(i))*(tt-tim(i))
               uy=uuy(i)+(uuy(i+1)-uuy(i))/(tim(i+1)-tim(i))*(tt-tim(i))
               ii=i
               ux=ux*accmult
               uy=uy*accmult
            endif        
            endif
          enddo

          if(ii.eq.0)then
            ux=0.
            uy=0.
          endif
      
      else
        ux=0.
        uy=0.
      endif

      call cstiffness(n,nnmax,nelmax,nel,nf,elmat,
     +  xcoord,elnode,loads,u,v,steel,As,damage,steeln,
     +  eltype,strainm,beta,Es,ey,eult,sult,
     +  epscr,a1,a2,a3,b1,b2,dsize,Rxn)

      if(phase.eq.2)then

          if(externalloadtype.eq.2)then

              if(rr(nvcont1).gt.vdesired)then
                rhs=-lambda*(rr(nvcont1)-vdesired)
              else
                rhs=-lambda*(rr(nvcont1)-vdesired)
              endif
              f=f+dt*frate+
     +                beta1*rhsold+
     +                beta2*rhs
              frate=frate+gamma1*rhsold+gamma2*rhs
              rhsold=rhs
              dtfactor=f-fold
              fold=f

          endif

          do i=1,nloads
                   
            if(externalloadtype.eq.1)then
                  
               do ii=1,loadingvaluelines(i)-1
                 t1=loadingvalue(i,ii,1)
                 t2=loadingvalue(i,ii+1,1)
                 f1=loadingvalue(i,ii,2)
                 f2=loadingvalue(i,ii+1,2)
                 if(tt.ge.t1)then
                 if(tt.le.t2)then
                    ff=f1+(f2-f1)/(t2-t1)*(tt-t1)
                    iii=ii
                 endif        
                 endif
               enddo

               if(iii.eq.0)then
                 ff=0.
               endif
            endif
            
            if(externalloadtype.eq.2)then
                if(i.ne.nvcont2)then
                    ff=0.0
                else
                    ff=f
                endif
            endif
            
            if(loadingtype(i).eq.1)then            
              do k=1,loadinglines(i)
                do j=loading(i,k,1),loading(i,k,2),loading(i,k,3)
                   i1=nf(j,abs(loading(i,k,4)))
                   if(i1.ne.0)then
                      loads(i1)=loads(i1)+loading(i,k,4)*ff
                   endif
                enddo
              enddo
            endif 

c          sum=0.
            if(loadingtype(i).eq.2)then
              do j=1,ntri(i)
                 i1=nf(nntri(i,j),1)
c                 write(11,*) xcoord(nntri(i,j),2),triff(i,j)
                 if(i1.ne.0)then
                    loads(i1)=loads(i1)+triff(i,j)*ff*trif(i,3)
c                     loads(i1)=loads(i1)+10000.
c                     write(*,*) i1,nntri(i,j)
                 endif
                 i1=nf(nntri(i,j),2)
                 if(i1.ne.0)then
                    loads(i1)=loads(i1)+triff(i,j)*ff*trif(i,4)
                 endif                                        
              enddo        
            endif     
          enddo
c          pause
c      write(73,*) sum,Rxn(1)
      
          if(readacc.eq.1.and.phase.eq.2)then
              do i=1,nn
                do j=1,2
                    if(nf(i,j).ne.0)then
                        if(j.eq.1)then
                          loads(nf(i,j))=loads(nf(i,j))-mm(nf(i,j))*ux
                        else
                          loads(nf(i,j))=loads(nf(i,j))-mm(nf(i,j))*uy
                        endif
                    endif
                enddo
              enddo      
          endif
      endif
            
      if(phase.eq.1)then
        gravity=tt/tphase1*gravityo
        if(gravity.gt.gravityo)gravity=gravityo
      endif
      if(phase.eq.2)gravity=gravityo

c      write(*,*) gravity,mass
      j=2
      do i=1,nn
        if(nf(i,j).ne.0)then
            loads(nf(i,j))=loads(nf(i,j))-mass*gravity
        endif
      enddo

      do i=1,nn
      k=0
        do j=1,nodeelement(i,1)
            if(damage(nodeelement(i,j+1)).eq.0)then
                k=k+1        
            endif
        enddo
        damagenode(i)=(k*1.0)/nodeelement(i,1)
      enddo

      do i=1,nn
        do j=1,2
            if(nf(i,j).ne.0)damagedof(nf(i,j))=damagenode(i)
        enddo
      enddo

      do i=1,n
        loads(i)=loads(i)-damping(alpha,damagedof(i))*mm(i)*v(i)
      enddo       
c     
c     loads = (F - Ku - Cv)/mass
c
      maxa=0.
      averagea=0.
      do i=1,n
        loads(i)=loads(i)/mm(i)
        averagea=averagea+loads(i)
        if(dabs(loads(i)).ge.maxa)maxa=dabs(loads(i))
      enddo      
      averagea=averagea/n
      
      

      do i=1,nn
        do j=1,2
            if(nfm(i,j).eq.0)then
                if(nf(i,j).ne.0)then
                    k=nf(i,j) 
        u(k)=u(k)+dt*v(k)+beta1*loadsold(k)+beta2*loads(k)
        v(k)=v(k)+gamma1*loadsold(k)+gamma2*loads(k)
        loadsold(k)=loads(k)                                     
                endif
            else
                if((tt-tthold).lt.0.02)then
                if(nf(i,j).ne.0)then
                   k=nf(i,j)                  
                    v(k)=vboundary
                    u(k)=u(k)+v(k)*dt
                endif
                else
                if(nf(i,j).ne.0)then
                   k=nf(i,j)                  
                    v(k)=0.
                    u(k)=u(k)+v(k)*dt
                endif  
                endif                  
            endif
        enddo
      enddo   
      
      if((tt-tthold).gt.0.04)then
        tthold=tt
      endif

      maxv=0.
      maxu=0.
      do i=1,nn
      do j=1,2
        if(nf(i,j).ne.0)then
            if(j.eq.1)then
                vx(i)=v(nf(i,1))
                dispx(i)=u(nf(i,1))
                accx(i)=loads(nf(i,1))
            else
                vy(i)=v(nf(i,2))
                dispy(i)=u(nf(i,2))
                accy(i)=loads(nf(i,2))    
            endif
         else
            if(j.eq.1)then
                vx(i)=0.
                dispx(i)=0.
                accx(i)=0.
            else
                vy(i)=0.
                dispy(i)=0.
                accy(i)=0.
            endif
         endif                        
        vvv(i)=dsqrt(vx(i)*vx(i)+vy(i)*vy(i))
        uuu(i)=dsqrt(dispx(i)*dispx(i)+dispy(i)*dispy(i))        
        if(uuu(i).ge.maxu)then
            maxu=uuu(i)
        endif
        if(vvv(i).ge.maxv)then
            maxv=vvv(i)
        endif
      enddo
      enddo

      k=0
      do i=1,nn
        do j=1,2
           k=k+1
           if(nf(i,j).eq.0)then
              uprint(k)=0.
              xcoord(i,j)=ycoord(i,j)
           else
              uprint(k)=u(nf(i,j))
              xcoord(i,j)=ycoord(i,j)+u(nf(i,j))
          endif
      enddo
      enddo
   
      nfailed=0
      do i=1,nel
        if(damage(i).eq.1)nfailed=nfailed+1
      enddo
      if(mod(ikt,pfreq2).eq.0)then
        averagea=dabs(averagea)
        write(*,876) ikt,tt,maxu,maxv,maxa,-Rxn(1),-Rxn(2),f,nfailed
      endif
876   format(i8,7(1pe12.3),i6)   
          
      nfailedold=nfailed

      call cstrain(nel,nnmax,nelmax,elnode,uprint,elmat,xcoord,
     +  strain,eltype)
  
      if(phase.eq.2)then
      if(mod(ikt,pfreq).eq.0.or.ikt.eq.1)then
      call printx(nel,nn,nnmax,nelmax,ycoord,uprint,elmat,vvv,
     +  vx,vy,accx,accy,dispx,dispy,strainm,damagenode,
     +  uuu,elnode,damage,strain,forces,nf,ikt,As,steel,amp)
      endif
      endif


      do i=1,nmonitor
        if(monitor(i,3).eq.1)then        
          rr(i)=u(nf(monitor(i,1),monitor(i,2)))
        endif
        if(monitor(i,3).eq.2)then
          if(monitor(i,2).eq.1)then
            rr(i)=vx(monitor(i,1))
          else
            rr(i)=vy(monitor(i,1))
          endif       
        endif
        if(monitor(i,3).eq.3)then
          if(monitor(i,2).eq.1)then
            rr(i)=accx(monitor(i,1))
          else
            rr(i)=accy(monitor(i,1))
          endif       
        endif        
      enddo        

      do i=1,nmonitor2
        if(monitor2(i,4).eq.1)then
            if(monitor2(i,3).eq.1)then
                t1=dispx(abs(monitor2(i,1)))
     +              *monitor2(i,1)/abs(monitor2(i,1))
                t2=dispx(abs(monitor2(i,2)))
     +              *monitor2(i,2)/abs(monitor2(i,2))
                rr(i+nmonitor)=t1+t2
            else
                t1=dispy(abs(monitor2(i,1)))
     +              *monitor2(i,1)/abs(monitor2(i,1))
                t2=dispy(abs(monitor2(i,2)))
     +              *monitor2(i,2)/abs(monitor2(i,2))
                rr(i+nmonitor)=t1+t2          
            endif     
        endif
        if(monitor2(i,4).eq.2)then
            if(monitor2(i,3).eq.1)then
                t1=vx(abs(monitor2(i,1)))
     +              *monitor2(i,1)/abs(monitor2(i,1))
                t2=vx(abs(monitor2(i,2)))
     +              *monitor2(i,2)/abs(monitor2(i,2))
                rr(i+nmonitor)=t1+t2
            else
                t1=vy(abs(monitor2(i,1)))
     +              *monitor2(i,1)/abs(monitor2(i,1))
                t2=vy(abs(monitor2(i,2)))
     +              *monitor2(i,2)/abs(monitor2(i,2))
                rr(i+nmonitor)=t1+t2         
            endif        
        endif
        if(monitor2(i,4).eq.3)then
            if(monitor2(i,3).eq.1)then
                t1=accx(abs(monitor2(i,1)))
     +              *monitor2(i,1)/abs(monitor2(i,1))
                t2=accx(abs(monitor2(i,2)))
     +              *monitor2(i,2)/abs(monitor2(i,2))
                rr(i+nmonitor)=t1+t2
            else
                t1=accy(abs(monitor2(i,1)))
     +              *monitor2(i,1)/abs(monitor2(i,1))
                t2=accy(abs(monitor2(i,2)))
     +              *monitor2(i,2)/abs(monitor2(i,2))
                rr(i+nmonitor)=t1+t2          
            endif         
        endif
      enddo
 

      if(mod(ikt,pfreq3).eq.0)then

      do i=1,nmonitor
        if(monitor(i,3).eq.1)then        
          write(30+i,*) tt,rr(i)
        endif
        if(monitor(i,3).eq.2)then
          if(monitor(i,2).eq.1)then
            write(30+i,*) tt,rr(i)
          else
            write(30+i,*) tt,rr(i)
          endif       
        endif
        if(monitor(i,3).eq.3)then
          if(monitor(i,2).eq.1)then
            write(30+i,*) tt,rr(i)
          else
            write(30+i,*) tt,rr(i)
          endif       
        endif        
      enddo        

      do i=1,nmonitor2
        if(monitor2(i,4).eq.1)then
            if(monitor2(i,3).eq.1)then
                write(30+nmonitor+i,*) tt,rr(i+nmonitor)
            else
                write(30+nmonitor+i,*) tt,rr(i+nmonitor)            
            endif     
        endif
        if(monitor2(i,4).eq.2)then
            if(monitor2(i,3).eq.1)then
                write(30+nmonitor+i,*) tt,rr(i+nmonitor)
            else
                write(30+nmonitor+i,*) tt,rr(i+nmonitor)
            endif        
        endif
        if(monitor2(i,4).eq.3)then
            if(monitor2(i,3).eq.1)then
                write(30+nmonitor+i,*) tt,rr(i+nmonitor)
            else

                write(30+nmonitor+i,*) tt,rr(i+nmonitor)            
            endif         
        endif
      enddo
      
      do i=1,nbase         
         write(30+i+nmonitor+nmonitor2,*) rr(base(i,2)),
     +  -Rxn(abs(base(i,1)))*base(i,1)/abs(base(i,1))
      enddo
      
      write(666,*) tt,-rxn(1)
      write(667,*) tt,-rxn(2)

      endif

      if(tt.ge.tphase1.and.phase.eq.1.and.maxa.lt.gravityacclimit)then
        phase=2
        ikt=0
        tt=0.0
        write(*,*) 'phase ',2
      endif

      if(phase.eq.2.and.tt.ge.ttmax)then
        write(*,*) 'completed the simulation'
        stop
      endif
      
      goto 241
      
      end

 